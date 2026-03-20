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

            var thisTeam = aiTeam.FirstOrDefault();
            AiTeam = await _battlePokemonFactory.CreateBattleTeam(thisTeam.AiPokemon);

            CurrentAIPokemon = AiTeam.First();
            await GetCurrentAiMoves();

            result.PlayerCurrentPokemon = CurrentPlayerPokemon;
            result.AiCurrentPokemon= CurrentAIPokemon;

            result.AiParty = AiTeam;
            result.PlayerParty = PlayerTeam;

            return result;
        }

       

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
        public async Task<TurnResult> NewTurn(string? playerMove)
        {
            await GetCurrentPlayerMoves();
            await GetCurrentAiMoves();
            TurnResult turnResult = new TurnResult();
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


            var opponentSecoundType = CurrentAIPokemon.PartyPokemon.Types.LastOrDefault();
            if (opponentSecoundType == CurrentAIPokemon.PartyPokemon.Types.First())
            {
                opponentSecoundType = null;
            }

            MoveModel aiMove = new MoveModel();
            if (!CurrentAIPokemon.IsFainted)
            {
                aiMove = await _aIService.AIChoosesMove(CurrentPlayerPokemon, CurrentAIPokemon);
            }
            ////////////
            ///Avgör vem som går först
            ///
            var moveOrder =  WhoPeformesActionFirst(move, aiMove);

            //Är player först
            if (moveOrder.first == CurrentPlayerPokemon)
            {
                await ExecuteMove(moveOrder.first, move, turnResult);
            }

            //Är AI först
            else if (moveOrder.first == CurrentAIPokemon)
            {
                await ExecuteMove(moveOrder.first, aiMove, turnResult);
            }

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

            //Är fiende vid liv
            if (moveOrder.secound.IsFainted==false)
            {
                //Är spelare vid liv
                if (moveOrder.secound == CurrentPlayerPokemon)
                {
                    await ExecuteMove(moveOrder.secound, move, turnResult);
                }
                //Är Ai vid liv
                else if (moveOrder.secound == CurrentAIPokemon)
                {
                    await ExecuteMove(moveOrder.secound, aiMove, turnResult);
                }
            }
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
            //Vem blir slagen
            int accuracyCheck = await _battleService.GetAccuracyCheck();
            bool itsaCrit = await _battleService.GetCritChange();
            Console.WriteLine(accuracyCheck);
            
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
           
            //MISS! och skippa damage
            if (accuracyCheck > move.Accuracy)
            {
                result.BattleActionMessages.Add("But it missed!");
                return;
            }
            if (!string.IsNullOrEmpty(effectivenssStatus))
            {
                result.BattleActionMessages.Add(effectivenssStatus);
            }
            //
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
            if (move.Priority > aiMove.Priority)
            {
                return (CurrentPlayerPokemon, CurrentAIPokemon);
            }
             if (move.Priority < aiMove.Priority)
            {
                return (CurrentAIPokemon, CurrentPlayerPokemon);
            }

            if (CurrentPlayerPokemon.PartyPokemon.Stats.Speed > CurrentAIPokemon.PartyPokemon.Stats.Speed)
            {
                return (CurrentPlayerPokemon, CurrentAIPokemon);
            }
            if(CurrentPlayerPokemon.PartyPokemon.Stats.Speed < CurrentAIPokemon.PartyPokemon.Stats.Speed)
            {
                return (CurrentAIPokemon, CurrentPlayerPokemon);
            }

            //Samma speed, välj randomly, föredra spelaren för demo purpose
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

        public async Task<ImageSource> LoadPokemonFrontSpritePathAsync(string name, string version)
        {
            return await _imageService.GetPokemonSpriteAsyncPNG(name, version);
        }

        internal async Task<ImageSource> LoadPokemonBackSpritePathAsync(string name, string version)
        {
            return await _imageService.GetPokemonBackSpriteAsyncPNG(name, version);
        }

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
