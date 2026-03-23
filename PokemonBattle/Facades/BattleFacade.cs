using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using Domain.Calculator;
using Domain.Models.Game;
using Domain.Services;
using Pokemon.AppServices.Factories;
using Pokemon.AppServices.Interfaces;
using Pokemon.AppServices.Interfaces.AI;
using Pokemon.AppServices.Models;
using Pokemon.ContractDTOs.Interfaces;
using Pokemon.Infrastructure.Factories;
using Pokemon.Infrastructure.Interfaces;
using Pokemon.Shared.Extensions;
using PokemonBattle.Factories;
using PokemonBattle.ListModel;

namespace PokemonBattle.Facades
{
    public class BattleFacade
    {
        private BattlePokemonFactory _battlePokemonFactory;
        private ListMoveModelFactory _moveModelFactory;
        private TypeDataService _typeDataService;
        private IFetchService _pokemonFetchRepository;
        private ITeamPokemonService _teamPokemonService;
        private IImageService _imageService;
        private IMoveService _moveService;
        private DamageCalculator _damageCalculator;
        private IAIService _aIService;
        private IAiTeamService _aiTeamService;
        private readonly IBattleService _battleService;
        private TypeModelFactory _typeModelFactory;
       
        public List<BattlePokemonModel> PlayerTeam {  get; set; } = new List<BattlePokemonModel>();
        public List<BattlePokemonModel> AiTeam { get; set; }

        public bool PlayerWin => AiTeam.All(p => p.IsFainted);
        public bool AiWin => PlayerTeam.All(p => p.IsFainted);

        public BattlePokemonModel CurrentPlayerPokemon { get; set; }
        public BattlePokemonModel CurrentAIPokemon { get; set; }

        public event Action? OnPlayerMustSwitch;
        public BattleFacade
            (
            IJsonStorage jsonStorage,
            IBattleService battleService,
            TypeDataService typeDataService,
            IFetchService pokemonFetchRepository,
            ITeamPokemonService teamPokemonService,
            DamageCalculator damageCalculator,
            IAIService aIService,
            IAiTeamService aiTeamService,
            IImageService imageService,
            IMoveService moveService,
            BattlePokemonFactory battlePokemonFactory,
            TypeModelFactory typeModelFactory,
            ListMoveModelFactory listMoveModelFactory
            
            )
        {

            //Yup, thats a load of services
            _typeModelFactory = typeModelFactory;
            _moveService = moveService;
            _moveModelFactory = listMoveModelFactory;
            _battlePokemonFactory = battlePokemonFactory;
            _battleService = battleService;
            _imageService = imageService;
            _aiTeamService= aiTeamService;
            _aIService = aIService;
            _typeDataService = typeDataService;
            _pokemonFetchRepository = pokemonFetchRepository;
            _teamPokemonService = teamPokemonService;
            _damageCalculator = damageCalculator;
        }
        //Start the match, get first pokemon and then moves
        public async Task<TurnResult> StartMatch()
        {
            TurnResult result = new TurnResult();
            var playerTeam =  _teamPokemonService.TeamPokemon.ToList();
            PlayerTeam = await _battlePokemonFactory.CreateBattleTeam(playerTeam);
            CurrentPlayerPokemon = PlayerTeam.First();

            await GetCurrentPlayerMoves();
            var pokemon = await _teamPokemonService.GetFirstPartyPokemon();
            //AI får sin egna senare
            var aiTeam = await _aiTeamService.GetTeams();
            if( aiTeam == null )
            {
                return null;
            }

            //Could use another method, random available team, first or default was enough for demo purposes
            var thisTeam = aiTeam.FirstOrDefault();
            AiTeam = await _battlePokemonFactory.CreateBattleTeam(thisTeam.AiPokemon);


            //This is just to reload type data
            List<string> types = new List<string>();
            List<TypeModel> enemyTypes = new List<TypeModel>();
            foreach (var pookemon in AiTeam)
            {
                types.AddRange(pookemon.PartyPokemon.Types);
            }

            //This makes sure the AI teams pokemon types are properly loaded and they can calculate properly
            foreach(var type in types)
            {
                var basicType = await _pokemonFetchRepository.GetTypeModelAsync(type);
                var typeData = _typeModelFactory.Create(basicType);
                enemyTypes.Add(typeData);
            }
            _typeDataService.AddTypes(enemyTypes.ToArray());
            CurrentAIPokemon = AiTeam.First();
            await GetCurrentAiMoves();

            //This just handles standard return of turnresult
            result.PlayerCurrentPokemon = CurrentPlayerPokemon;
            result.AiCurrentPokemon= CurrentAIPokemon;

            result.AiParty = AiTeam;
            result.PlayerParty = PlayerTeam;

            return result;
        }

       
        //We get and int from the frontend, which is same index order as the internal game team
        public async Task<BattlePokemonModel> ChangeCurrentPokemon(int playerPokemon)
        {
            if(playerPokemon < 0 || playerPokemon >= PlayerTeam.Count)
            {
                throw new ArgumentOutOfRangeException("Index was out of bounds of player team");
            }
            var thisTeamMember = PlayerTeam[playerPokemon];

            CurrentPlayerPokemon = thisTeamMember;
            return CurrentPlayerPokemon;

        }

        //every new turn
        public async Task<TurnResult> NewTurn(string? playerMove)
        {
            await GetCurrentPlayerMoves();
            await GetCurrentAiMoves();
            TurnResult turnResult = new TurnResult();

            //Handle moves that are somehow how empty, end the player action early
            if (string.IsNullOrEmpty(playerMove))
            {
                //Player has hopefully switched pokemon
                //Jump to Ai part
                var aiMovesFirst = await _aIService.AIChoosesMove(CurrentPlayerPokemon, CurrentAIPokemon);
                await ExecuteMove(CurrentAIPokemon, aiMovesFirst, turnResult);
                turnResult.PlayerCurrentPokemon = CurrentPlayerPokemon;
                turnResult.AiCurrentPokemon = CurrentAIPokemon;
                turnResult.PlayerParty = PlayerTeam;
                turnResult.AiParty = AiTeam;

                return turnResult;
            }
            //Kolla vilket move det är i CurrentPlayerMove
            var move = CurrentPlayerPokemon.Moves.FirstOrDefault(x=>x.Name.ToLower() == playerMove.ToLower());
            var moveWithType = await _pokemonFetchRepository.GetMoveModelAsync(move.Name);
            var actualMoveType =   _typeDataService.GetTypeModel(moveWithType.MoveTypeInfo.Name);
            move.Type = actualMoveType;
            var damage = _damageCalculator.CalculatDamage(CurrentPlayerPokemon.PartyPokemon, CurrentAIPokemon.PartyPokemon, move);


            //Makes sure no Fire/Fire type
            var opponentSecoundType = CurrentAIPokemon.PartyPokemon.Types.LastOrDefault();
            if (opponentSecoundType == CurrentAIPokemon.PartyPokemon.Types.First())
            {
                opponentSecoundType = null;
            }

            //Ai gets to choose move
            MoveModel aiMove = new MoveModel();
            if (!CurrentAIPokemon.IsFainted)
            {
                aiMove = await _aIService.AIChoosesMove(CurrentPlayerPokemon, CurrentAIPokemon);
            }
            ////////////
            ///Who moves first, most of the time its the fastest
            ///
            var moveOrder =  WhoPeformesActionFirst(move, aiMove);

            //Player first
            if (moveOrder.first == CurrentPlayerPokemon)
            {
                await ExecuteMove(moveOrder.first, move, turnResult);
            }

            //Ai first
            else if (moveOrder.first == CurrentAIPokemon)
            {
                await ExecuteMove(moveOrder.first, aiMove, turnResult);
            }


            //Handle player logic to switch
            if (CurrentPlayerPokemon.IsFainted)
            {
                turnResult.PlayerFainted = true;

                OnPlayerMustSwitch?.Invoke();


                turnResult.PlayerCurrentPokemon = CurrentPlayerPokemon;
                turnResult.AiCurrentPokemon = CurrentAIPokemon;
                turnResult.PlayerParty = PlayerTeam;
                turnResult.AiParty = AiTeam;
                turnResult.AiWin = AiWin;
                return turnResult;
            }
            //Ai is simpler, but give the green flag to change pokemon

            if (CurrentAIPokemon.IsFainted)
            {
                turnResult.AiFainted = true;
                turnResult.PlayerCurrentPokemon = CurrentPlayerPokemon;
                turnResult.AiCurrentPokemon = CurrentAIPokemon;
                turnResult.PlayerParty = PlayerTeam;
                turnResult.AiParty = AiTeam;
                turnResult.PlayerWin = PlayerWin;
                return turnResult;
            }

            //Is the slower pokemon alive
            if (moveOrder.secound.IsFainted==false)
            {
                //Player
                if (moveOrder.secound == CurrentPlayerPokemon)
                {
                    await ExecuteMove(moveOrder.secound, move, turnResult);
                }
                //AI
                else if (moveOrder.secound == CurrentAIPokemon)
                {
                    await ExecuteMove(moveOrder.secound, aiMove, turnResult);
                }
            }

            //Return result after both have attacked
            turnResult.PlayerCurrentPokemon = CurrentPlayerPokemon;
            turnResult.AiCurrentPokemon = CurrentAIPokemon;
            turnResult.PlayerParty = PlayerTeam;
            turnResult.AiParty = AiTeam;
            turnResult.PlayerWin = PlayerWin;
            turnResult.AiWin = AiWin;

            return turnResult;
          
        }
        private async Task ExecuteMove(BattlePokemonModel attacker, MoveModel move, TurnResult result)
        {
            //Accuracy roll, if moves accuracy is below roll, MISS!
            int accuracyCheck = await _battleService.GetAccuracyCheck();

            //random roll to crit
            bool itsaCrit = await _battleService.GetCritChange();
            Console.WriteLine(accuracyCheck);


            //Vem blir slagen
            //Defender är AI om attacker är Player, annars är AI attacker
            BattlePokemonModel defender = attacker == CurrentPlayerPokemon ? CurrentAIPokemon : CurrentPlayerPokemon;
            if (defender.IsFainted)
            {
                return;
            }
            result.BattleActionMessages.Add($"{attacker.NicknameOrName} used {move.Name}!");
            //Räkna hur mycket
            var damage = _damageCalculator.CalculatDamage(attacker.PartyPokemon, defender.PartyPokemon, move);

            string effectivenssStatus = await _battleService.GetEffectivnessStatus(damage.multiplier); 
           
            //MISS!
            if (accuracyCheck > move.Accuracy)
            {
                result.BattleActionMessages.Add("But it missed!");
                return;
            }
            if (!string.IsNullOrEmpty(effectivenssStatus))
            {
                result.BattleActionMessages.Add(effectivenssStatus);
            }
            //Crit logic, get crit modifier which could be changed in the future to replicate different games (sometimes it 1.5x, sometimes 2x damage)
            if (itsaCrit)
            {
                result.BattleActionMessages.Add("It's a critical hit!");
                var critMOdifier = await _battleService.GetCritModifier();
                var newDamage = damage.damage * critMOdifier;
                damage.damage = (int)Math.Round(newDamage);

            }
            

            double damageRoll = await _battleService.GetDamageRoll();
            double damageWithRoll = damage.damage*damageRoll;
            damage.damage = (int)Math.Round(damageWithRoll);


            result.BattleActionMessages.Add($"{defender.NicknameOrName} took {damage} damage!");


            defender.TakeDamage(damage.damage);
            if (defender.IsFainted)
            {
                result.BattleActionMessages.Add($"{defender.NicknameOrName} fainted!");
            }

            await Task.CompletedTask;
        }

        private (BattlePokemonModel first, BattlePokemonModel secound) WhoPeformesActionFirst(MoveModel move, MoveModel aiMove)
        {
            //Quick attack goes first
            if (move.Priority > aiMove.Priority)
            {
                return (CurrentPlayerPokemon, CurrentAIPokemon);
            }
             if (move.Priority < aiMove.Priority)
            {
                return (CurrentAIPokemon, CurrentPlayerPokemon);
            }

             //Fastest first
            if (CurrentPlayerPokemon.PartyPokemon.Stats.Speed > CurrentAIPokemon.PartyPokemon.Stats.Speed)
            {
                return (CurrentPlayerPokemon, CurrentAIPokemon);
            }
            if(CurrentPlayerPokemon.PartyPokemon.Stats.Speed < CurrentAIPokemon.PartyPokemon.Stats.Speed)
            {
                return (CurrentAIPokemon, CurrentPlayerPokemon);
            }

            //Same speed, random roll
            int random = Random.Shared.Next(2);
            if (random == 0)
            {
                return (CurrentPlayerPokemon, CurrentAIPokemon);
            }
            if(random == 1)
            {
                return (CurrentAIPokemon, CurrentPlayerPokemon);
            }
            else
            {
                return (CurrentPlayerPokemon, CurrentAIPokemon);
            }
        }

        //Just get the next pokemon
        public async Task<BattlePokemonModel>? AIChoosesNewPokemon()
        {
             await Task.Delay(1000);
            int currentAiPokemonIndex = AiTeam.IndexOf(CurrentAIPokemon);

            for(int i = 0; i<AiTeam.Count; i++)
            {
                var aiPokemon = AiTeam[i];
                if (!aiPokemon.IsFainted)
                {
                    CurrentAIPokemon = aiPokemon;
                    return CurrentAIPokemon;
                }
            }
            //Well nu har AI inge pokemon kvar
            //throw new Exception("AI has no more usable pokemon left! Player wins?!");
            return null;

        }

        //Returns imgs
        public async Task<ImageSource> LoadPokemonFrontSpritePathAsync(string name, string version)
        {
            return await _imageService.GetPokemonSpriteAsyncPNG(name, version);
        }

        internal async Task<ImageSource> LoadPokemonBackSpritePathAsync(string name, string version)
        {
            return await _imageService.GetPokemonBackSpriteAsyncPNG(name, version);
        }

        //Load moves for UI
        public async Task<ObservableCollection<ListMoveDisplayModel>> GetCurrentPlayerMoves()
        {
            var baseMoves = CurrentPlayerPokemon.PartyPokemon.Moves.ToList();

            CurrentPlayerPokemon.Moves = await _moveService.GetMoveModels(baseMoves);

            return await _moveModelFactory.CreateList(CurrentPlayerPokemon.Moves);
        }
        private async Task GetCurrentAiMoves()
        {
            var baseMoves = CurrentAIPokemon.PartyPokemon.Moves.ToList();
            CurrentAIPokemon.Moves = await _moveService.GetMoveModels(baseMoves);
        }

    }
}
