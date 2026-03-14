using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;

namespace Domain.Interface
{
    public interface IStatCalculator
    {
        //Alla calculators ska göra detta
        //Fast beroende på generation, använd olika ekvationer
        //Jag användar bara gen 3 för fireread & leafgreen
        StatModel CalculateEffectiveStats(StatModel baseStat);
    }
}
