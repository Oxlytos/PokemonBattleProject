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
        //Räkna skada spelare kommer göra
        //AI loopar över hela dena metod för att hitta bästa attacken
        //Har AI:et 4 attacker, kör den fyra gånger, använd bästa attacken
        public (int damage, double multiplier) CalculatDamage(PartyPokemonModel attacker, PartyPokemonModel defender, MoveModel move)
        {

            var firstDefenderType = defender.Types.FirstOrDefault();
            var secoundDefenderType = defender.Types.LastOrDefault();

            if (firstDefenderType == secoundDefenderType)
            {
                secoundDefenderType = null;
            }

            double damageMultiploer = _typeDataService.GetTypeAttackMultiplier(move.Type.Name, firstDefenderType, secoundDefenderType);


            //Fire is special
            //Fighting physical
            //Gen 4 implemented the big physical/special split, but not gen 3 (which this handles)
            int attackStat;
            int defenseStat;
            if (move.Type.IsSpecialDamage)
            {
                attackStat = attacker.Stats.SpecialAttack;
                defenseStat = defender.Stats.SpecialDefense;
            }
            else
            {
                attackStat = attacker.Stats.Attack;
                defenseStat = defender.Stats.Defense;
            }

            //Buff moves has no damage value for now, as implenting the buff aspect modifier is a ways away
            float movePower = move.Power ?? 0;
            int power = (int)movePower;

            double baseDamage = (((double)attackStat/(double)defenseStat) * power);
            baseDamage*=damageMultiploer;

            //STAB damage, if a fire type uses a fire type move, a FLAT 50% bonus 
            if (attacker.Types.Contains(move.Type.Name))
            {
                baseDamage *= 1.5;
            }

            //Return the damage (important) and the modifier (less importnat, just for printing "It's not very effective etc"
            return(((int)(baseDamage)), (double)(damageMultiploer));

        }
    }
}
