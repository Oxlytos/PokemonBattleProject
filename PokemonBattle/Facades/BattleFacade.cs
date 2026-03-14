using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Calculator;
using Domain.Models.Game;
using Domain.Services;
using Pokemon.Infrastructure.Interfaces;
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


        public PartyPokemonModel CurrentPlayerPokemon { get; set; }
        public PartyPokemonModel CurrentAIPokemon { get; set; }

        public BattleFacade
            (
            IJsonStorage jsonStorage,
            IBattleService battleService,
            TypeDataService typeDataService,
            IPokemonFetchRepository pokemonFetchRepository,
            ITeamPokemonService teamPokemonService,
            DamageCalculator damageCalculator
            )
        {
            _jsonStorage = jsonStorage;
            _battleService = battleService;
            _typeDataService = typeDataService;
            _pokemonFetchRepository = pokemonFetchRepository;
            _teamPokemonService = teamPokemonService;
            _damageCalculator = damageCalculator;
        }
        public async Task StartMatch()
        {

            CurrentPlayerPokemon = await _teamPokemonService.GetFirstPartyPokemon();
            //AI får sin egna senare
            CurrentAIPokemon = await _teamPokemonService.GetFirstPartyPokemon();

        }
        public async Task NewTurn(string playerMove)
        {
            //Kolla vilket move det är i CurrentPlayerMove
            var move = CurrentPlayerPokemon.Moves.FirstOrDefault(x=>x.Name == playerMove);


            //
            var damage = _damageCalculator.CalculatDamage(CurrentPlayerPokemon, CurrentAIPokemon, move);

            var opponentFirstType = CurrentAIPokemon.Types.FirstOrDefault();

            var opponentSecoundType = CurrentAIPokemon.Types.LastOrDefault();
            if (opponentSecoundType == CurrentAIPokemon.Types.First())
            {
                opponentSecoundType = null;
            }

            var damageMultiplier = _typeDataService.GetTypeAttackMultiplier(move.Type.Name, opponentFirstType, opponentSecoundType);

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


            ////////////
            ///Avgör vem som går först
            ///
            MoveModel aiMove = new MoveModel();

            var moveOrder =  WhoPeformesActionFirst(move, aiMove);

            await ExecuteMove(moveOrder.first, move);

            if (moveOrder.secound.Stats.Health != 0)
            {
                await ExecuteMove(moveOrder.secound, move);
            }
        }
        private async Task ExecuteMove(PartyPokemonModel partyPokemonModel, MoveModel move)
        {

        }

        private (PartyPokemonModel first, PartyPokemonModel secound) WhoPeformesActionFirst(MoveModel move, MoveModel aiMove)
        {
            if (move.Priority > aiMove.Priority)
            {
                return (CurrentPlayerPokemon, CurrentAIPokemon);
            }
             if (move.Priority < aiMove.Priority)
            {
                return (CurrentAIPokemon, CurrentPlayerPokemon);
            }

            if (CurrentPlayerPokemon.Stats.Speed > CurrentAIPokemon.Stats.Speed)
            {
                return (CurrentPlayerPokemon, CurrentAIPokemon);
            }
            if(CurrentPlayerPokemon.Stats.Speed < CurrentAIPokemon.Stats.Speed)
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
