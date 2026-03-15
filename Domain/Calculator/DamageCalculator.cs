using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Domain.Services;

namespace Domain.Calculator
{
    public class DamageCalculator
    {
        private readonly TypeDataService _typeDataService;
        public DamageCalculator(TypeDataService typeDataService)
        {
            _typeDataService = typeDataService;
        }
        public int CalculatDamage(PartyPokemonModel attacker, PartyPokemonModel defender, MoveModel move)
        {

            var firstDefenderType = defender.Types.FirstOrDefault();
            var secoundDefenderType = defender.Types.LastOrDefault();

            if (firstDefenderType == secoundDefenderType)
            {
                secoundDefenderType = null;
            }


            Console.WriteLine(move.Type.Name);
            double damageMultiploer = _typeDataService.GetTypeAttackMultiplier(move.Type.Name, firstDefenderType, secoundDefenderType);


            int attackStat = attacker.Stats.Attack;
            int defenseStat = defender.Stats.Defense;
            int power = (int)move.Power;

            double baseDamage = ((attackStat/defenseStat) * power);
            baseDamage*=damageMultiploer;

            return (int)Math.Round(baseDamage);
        }
    }
}
