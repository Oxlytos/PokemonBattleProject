using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Calculator;
using Domain.Models.Game;
using Domain.Services;
using Pokemon.Infrastructure.Factories;
using Pokemon.Infrastructure.Interfaces;
using Pokemon.Infrastructure.Interfaces.AI;
using Pokemon.Infrastructure.Models;
using Pokemon.Repository.Interfaces;
using PokemonBattle.ListModel;

namespace PokemonBattle.Facades
{
    public class BattleFacade
    {
        private IJsonStorage _jsonStorage;
        private IBattleService _battleService;
        private TypeDataService _typeDataService;
        private IPokemonFetchRepository _pokemonFetchRepository;
        private ITeamPokemonService _teamPokemonService;
        private DamageCalculator _damageCalculator;
        private IAIService _aIService;
        private IAiTeamService _aiTeamService;

        public List<BattlePokemonModel> PlayerTeam {  get; set; } = new List<BattlePokemonModel>();
        public List<BattlePokemonModel> AiTeam { get; set; }

        public BattlePokemonModel CurrentPlayerPokemon { get; set; }
        public BattlePokemonModel CurrentAIPokemon { get; set; }

        public event Action? OnPlayerMustSwitch;
        public BattleFacade
            (
            IJsonStorage jsonStorage,
            IBattleService battleService,
            TypeDataService typeDataService,
            IPokemonFetchRepository pokemonFetchRepository,
            ITeamPokemonService teamPokemonService,
            DamageCalculator damageCalculator,
            IAIService aIService,
            IAiTeamService aiTeamService
            
            )
        {
            _aiTeamService= aiTeamService;
            _aIService = aIService;
            _jsonStorage = jsonStorage;
            _battleService = battleService;
            _typeDataService = typeDataService;
            _pokemonFetchRepository = pokemonFetchRepository;
            _teamPokemonService = teamPokemonService;
            _damageCalculator = damageCalculator;
        }
        public async Task<TurnResult> StartMatch()
        {
            TurnResult result = new TurnResult();
            var playerTeam =  _teamPokemonService.TeamPokemon.ToList();
            PlayerTeam = BattlePokemonFactory.CreateBattleTeam(playerTeam);
            CurrentPlayerPokemon = PlayerTeam.First();
            var pokemon = await _teamPokemonService.GetFirstPartyPokemon();
            //AI får sin egna senare
            var aiTeam = await _aiTeamService.GetTeams();
            if( aiTeam == null )
            {
                return null;
            }

            var thisTeam = aiTeam.FirstOrDefault();
            AiTeam = BattlePokemonFactory.CreateBattleTeam(thisTeam.AiPokemon);
            CurrentAIPokemon = AiTeam.First();
            Console.WriteLine(CurrentAIPokemon);

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
            var move = CurrentPlayerPokemon.Moves.FirstOrDefault(x=>x.Name == playerMove);

            var moveWithType = await _pokemonFetchRepository.GetMoveModelAsync(move.Name);
            var actualMoveType =   _typeDataService.GetTypeModel(moveWithType.MoveTypeInfo.Name);
            move.Type = actualMoveType;
            //
            var damage = _damageCalculator.CalculatDamage(CurrentPlayerPokemon.PartyPokemon, CurrentAIPokemon.PartyPokemon, move);


            string aiFirstType = CurrentAIPokemon.PartyPokemon.Types.First();
            string aiSecondType = CurrentAIPokemon.PartyPokemon.Types.Last();




            var opponentFirstType = CurrentAIPokemon.PartyPokemon.Types.FirstOrDefault();

            var opponentSecoundType = CurrentAIPokemon.PartyPokemon.Types.LastOrDefault();
            if (opponentSecoundType == CurrentAIPokemon.PartyPokemon.Types.First())
            {
                opponentSecoundType = null;
            }


            ///////////////////
            ///AI del här senare

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
                return turnResult;
            }

            if (CurrentAIPokemon.IsFainted)
            {
                turnResult.AiFainted = true;
                turnResult.PlayerCurrentPokemon = CurrentPlayerPokemon;
                turnResult.AiCurrentPokemon = CurrentAIPokemon;
                turnResult.PlayerParty = PlayerTeam;
                turnResult.AiParty = AiTeam;
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

            return turnResult;
          
        }
        private async Task ExecuteMove(BattlePokemonModel attacker, MoveModel move, TurnResult result)
        {
            //Vem blir slagen
            BattlePokemonModel defender = attacker == CurrentPlayerPokemon ? CurrentAIPokemon : CurrentPlayerPokemon;
            if (defender.IsFainted)
            {
                return;
            }
            result.BattleActionMessages.Add($"{attacker.PartyPokemon.Nickname} used {move.Name}!");
            //Räkna hur mycket
            var damage = _damageCalculator.CalculatDamage(attacker.PartyPokemon, defender.PartyPokemon, move);

            string effectivenssStatus = GetEffectivnessStatus(damage.multiplier);
            if (!string.IsNullOrEmpty(effectivenssStatus))
            {
                result.BattleActionMessages.Add(effectivenssStatus);
            }

            result.BattleActionMessages.Add($"{defender.PartyPokemon.Nickname} took {damage} damage!");

            defender.TakeDamage(damage.damage);
            if (defender.IsFainted)
            {
                result.BattleActionMessages.Add($"{defender.PartyPokemon.Nickname} fainted!");
            }

            await Task.CompletedTask;
        }

        private string GetEffectivnessStatus(int damageMultiplier)
        {
            if(damageMultiplier == 0)
            {
                return "Had no effect...";
            }
            if (damageMultiplier < 1)
            {
                return "It was not very effective!";
            }
            if(damageMultiplier > 1)
            {
                return "It was super effective!";
            }
            return "";
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

        public async Task<BattlePokemonModel> AIChoosesNewPokemon()
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
            throw new Exception("AI has no more usable pokemon left! Player wins?!");
            return null;

        }
    }
}
