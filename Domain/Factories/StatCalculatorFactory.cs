using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Calculator;
using Domain.Interface;
using Domain.Models.Game;

namespace Domain.Factories
{
    public static class StatCalculatorFactory
    {
        public static IStatCalculator GetCalculator(int generation = 3)
        {
            //Retunera baserad på vilken generation
            return generation switch
            {
                3 => new GenerationThreeStatCalculator(), //Bara gen 3 för nu
                _ => throw new NotImplementedException(),
            };
        }
    }
}
