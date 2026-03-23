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
    public class TypeListMapper : ITypeMapper
    {
        //Returns string of requestmodels typings
        public List<string> MapTypes(RequestPokeonModel request)
        {
            List<string> typeModels = new List<string>();

            foreach (var type in request.Types)
            {
                typeModels.Add(type.Types.Name);
            }
            return typeModels;
        }
    }
}
