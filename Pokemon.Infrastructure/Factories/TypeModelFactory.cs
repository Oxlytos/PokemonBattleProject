using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Pokemon.ContractDTOs.RequestModel;
using Pokemon.Infrastructure.Interfaces;
using Pokemon.Infrastructure.Mapper;

namespace Pokemon.Infrastructure.Factories
{
    public class TypeModelFactory:ITypeModelFactory
    {
        public TypeModel Create(RequestTypeModel apiType)
        {
            //skapa objekt
            TypeModel model = new TypeModel();

            Console.WriteLine(apiType);

            //mappa ut allt
            model = TypeMapper.MapType(apiType);

            //retunera
            return model;
        }

     
    }
}
