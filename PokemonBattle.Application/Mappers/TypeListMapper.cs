using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Domain.Models.RequestModels;
using Pokemon.AppServices.Interfaces;

namespace Pokemon.AppServices.Mappers
{
    public class TypeListMapper : ITypeMapper
    {
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
