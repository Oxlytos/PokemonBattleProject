using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Domain.Models.RequestModels;

namespace Pokemon.AppServices.Mappers
{
    public static class TypeMapper
    {
        public static List<string> MapTypes(RequestPokeonModel request)
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
