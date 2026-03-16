using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Calculator;
using Domain.Models.Game;
using Pokemon.Infrastructure.Interfaces.AI;
using Pokemon.Infrastructure.Models;
using Pokemon.Services.Interfaces;

namespace Pokemon.Infrastructure.Services.AI
{
    public class AIService : IAIService
    {
        private readonly DamageCalculator _damageCalculator;
        private readonly IPokemonFetchService _pokeFetchService;
        public AIService(DamageCalculator damageCalculator, IPokemonFetchService pokemonFetchService)
        {
            _pokeFetchService = pokemonFetchService;
            _damageCalculator = damageCalculator;
        }
        //här får AI välja move
        public async Task<MoveModel> AIChoosesMove(BattlePokemonModel player, BattlePokemonModel aiPokemon)
        {
            //bestäm tidigt att vi ska hitta bästa move
            //har fienden bar 1 attack, det är dens bästa
            //har den 4 så tar den den med högst damagae (Power + Multiplíer)
            //Så fiende golem borde föredra att använda rock throw mot en Charizard (x4 multiplier) över double edge typ
            MoveModel bestMove = null;
            int bestPossibleDamage = 0;
            foreach (var move in aiPokemon.Moves)
            {
                var moveData = await _pokeFetchService.GetMoveModelAsync(move.Name);
                Console.WriteLine(moveData);
                move.Type.Name = moveData.MoveTypeInfo.Name;
                //gör kalk, hitta det som ger högst int
                var damage = _damageCalculator.CalculatDamage(aiPokemon.PartyPokemon, player.PartyPokemon, move);
                if(damage.damage > bestPossibleDamage)
                {
                    bestPossibleDamage = damage.damage;
                    bestMove = move;
                }
            }

            return bestMove;
        }
    }
}
