using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Calculator;
using Domain.Models.Game;
using Pokemon.AppServices.Interfaces;
using Pokemon.AppServices.Interfaces.AI;
using Pokemon.AppServices.Models;
using Pokemon.Infrastructure.Interfaces;

namespace Pokemon.AppServices.Services.AI
{
    public class AIService : IAIService
    {
        private readonly DamageCalculator _damageCalculator;
        private readonly IMoveService _moveService;
        public AIService(DamageCalculator damageCalculator, IMoveService moveService)
        {
            _moveService = moveService;
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
            List<string> moves = aiPokemon.Moves.Select(x => x.Name).ToList();

            Console.WriteLine(moves);
            List<MoveModel> movesWithData = new List<MoveModel>();
            Console.WriteLine(movesWithData);
            movesWithData = await _moveService.GetMoveModels(moves);

            Console.WriteLine(movesWithData);
            foreach (var move in movesWithData)
            {
                //gör kalk, hitta det som ger högst int
                Console.WriteLine(move);
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
