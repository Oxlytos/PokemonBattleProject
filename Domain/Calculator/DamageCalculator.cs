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


            //Eld attack använder special attack
            //Fighting fysisk
            //Så var det från gen 1-3
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

            //Buff moves har ingen power, ge den 0
            float movePower = move.Power ?? 0;
            int power = (int)movePower;

            double baseDamage = (((double)attackStat/(double)defenseStat) * power);
            baseDamage*=damageMultiploer;

            //STAB damage, om en eld pokemon använder en elda ttack får den en damage bonus
            if (attacker.Types.Contains(move.Type.Name))
            {
                baseDamage *= 1.5;
            }

            //Vi använder främst damagemultiploer för att printa "It's super effective!"
            return(((int)(baseDamage)), (double)(damageMultiploer));

        }
    }
}
