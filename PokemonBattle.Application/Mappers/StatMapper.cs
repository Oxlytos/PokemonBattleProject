using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Domain.Models.RequestModels;

namespace Pokemon.AppServices.Mappers
{
    public static class StatMapper
    {
        public static StatModel MapStats(RequestPokeonModel request)
        {
            StatModel model = new StatModel();
            request.SetBaseStatTotals();
            Console.WriteLine(request);
            model.Health = request.Health.Value;
            model.Attack = request.Attack.Value;
            model.Defense = request.Defense.Value;
            model.SpecialAttack = request.SpecialAttack.Value;
            model.SpecialDefense = request.SpecialDefense.Value;
            model.Speed = request.Speed.Value;
            return model;
        }
    }
}
