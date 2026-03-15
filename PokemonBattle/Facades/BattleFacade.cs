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
        public async Task<TurnResult> NewTurn(string playerMove)
        {
            TurnResult turnResult = new TurnResult();
            //Kolla vilket move det är i CurrentPlayerMove
            var move = CurrentPlayerPokemon.Moves.FirstOrDefault(x=>x.Name == playerMove);

            var moveWithType = await _pokemonFetchRepository.GetMoveModelAsync(move.Name);
            var actualMoveType =   _typeDataService.GetTypeModel(moveWithType.MoveTypeInfo.Name);
            move.Type = actualMoveType;
            //
            var damage = _damageCalculator.CalculatDamage(CurrentPlayerPokemon.PartyPokemon, CurrentAIPokemon.PartyPokemon, move);



            var opponentFirstType = CurrentAIPokemon.PartyPokemon.Types.FirstOrDefault();

            var opponentSecoundType = CurrentAIPokemon.PartyPokemon.Types.LastOrDefault();
            if (opponentSecoundType == CurrentAIPokemon.PartyPokemon.Types.First())
            {
                opponentSecoundType = null;
            }

            Console.WriteLine(moveWithType);
            Console.WriteLine(opponentFirstType);
            Console.WriteLine(opponentSecoundType);

            var damageMultiplier = _typeDataService.GetTypeAttackMultiplier(moveWithType.MoveTypeInfo.Name, opponentFirstType, opponentSecoundType);

            if (damageMultiplier == 0)
            {
                damage = 0;
            }

            var effectiveDamage = damageMultiplier*damage;

            damage = (int)effectiveDamage;

            if(damage > 0)
            {
               
            }

            ///////////////////
            ///AI del här senare

            MoveModel aiMove = new MoveModel();
            aiMove = await _aIService.AIChoosesMove(CurrentPlayerPokemon, CurrentAIPokemon);
            ////////////
            ///Avgör vem som går först
            ///

            var moveOrder =  WhoPeformesActionFirst(move, aiMove);

            //Är player först
            if (moveOrder.first == CurrentPlayerPokemon)
            {
                await ExecuteMove(moveOrder.first, move);
            }

            //Är AI först
            else if (moveOrder.first == CurrentAIPokemon)
            {
                await ExecuteMove(moveOrder.first, aiMove);
            }

            //Är fiende vid liv
            if (moveOrder.secound.PartyPokemon.Stats.Health != 0)
            {
                //Är spelare vid liv
                if (moveOrder.secound == CurrentPlayerPokemon)
                {
                    await ExecuteMove(moveOrder.secound, move);
                }
                //Är Ai vid liv
                else if (moveOrder.secound == CurrentAIPokemon)
                {
                    await ExecuteMove(moveOrder.secound, aiMove);
                }
            }
            turnResult.PlayerCurrentPokemon = CurrentPlayerPokemon;
            turnResult.AiCurrentPokemon = CurrentAIPokemon;
            turnResult.PlayerParty = PlayerTeam;
            turnResult.AiParty = AiTeam;

            return turnResult;
          
        }
        private async Task ExecuteMove(BattlePokemonModel attacker, MoveModel move)
        {
            //Vem blir slagen
            BattlePokemonModel defender = null;
            if (attacker == CurrentPlayerPokemon)
            {
                defender = CurrentAIPokemon;
            }
            else if (attacker == CurrentAIPokemon)
            {
                defender = CurrentPlayerPokemon;
            }

            //Räkna hur mycket
            int damage = _damageCalculator.CalculatDamage(attacker.PartyPokemon, defender.PartyPokemon, move);

            defender.TakeDamage(damage);

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
    }
}
