using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Pokemon.AppServices.Interfaces;
using Pokemon.ContractDTOs.RequestModel;

namespace Pokemon.AppServices.Mappers
{
    public class StatMapper : IStatMapper
    {
        public StatModel MapStats(RequestPokeonModel request)
        {
            StatModel model = new StatModel();
            request.SetBaseStatTotals();
            Console.WriteLine(request);
            model.BaseHealth = request.Health.Value;
            model.BaseAttack = request.Attack.Value;
            model.BaseDefense = request.Defense.Value;
            model.BaseSpecialAttack = request.SpecialAttack.Value;
            model.BaseSpecialDefense = request.SpecialDefense.Value;
            model.BaseSpeed = request.Speed.Value;
            return model;
        }
    }
}
