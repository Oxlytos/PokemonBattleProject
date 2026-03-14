using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interface;
using Domain.Models.Game;

namespace Domain.Calculator
{
    public class GenerationThreeStatCalculator : IStatCalculator
    {
        public StatModel CalculateEffectiveStats(StatModel baseStat)
        {

            baseStat.Health = HealthFormula(baseStat.BaseHealth);

            baseStat.Attack = OtherStatFormula(baseStat.BaseAttack);

            baseStat.Defense = OtherStatFormula(baseStat.BaseDefense);

            baseStat.SpecialAttack = OtherStatFormula(baseStat.BaseSpecialAttack);

            baseStat.SpecialDefense = OtherStatFormula(baseStat.BaseSpecialDefense);

            baseStat.Speed = OtherStatFormula(baseStat.BaseSpeed);



            return baseStat;

        }

        internal static int HealthFormula(int baseStat)
        {
            //50 är level
            //https://bulbapedia.bulbagarden.net/wiki/Stat#HP
            int effectiveHealthStat = ((2 * baseStat) * 50) / 100 + 50 + 10;
            return effectiveHealthStat;
        }

        internal static int OtherStatFormula(int baseStat)
        {
            int effectiveStat = ((2 * baseStat) * 50) / 100 + 5;
            return effectiveStat;
        }
    }
}
