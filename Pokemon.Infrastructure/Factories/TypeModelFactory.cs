using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Domain.Models.RequestModels;
using Pokemon.Infrastructure.Mappers;

namespace Pokemon.Infrastructure.Factories
{
    public static class TypeModelFactory
    {
        public static TypeModel Create(RequestTypeModel apiType)
        {
            //skapa objekt
            TypeModel model = new TypeModel();

            //mappa ut allt
            model = TypeMapper.MapType(apiType);

            //retunera
            return model;
        }
    }
}
