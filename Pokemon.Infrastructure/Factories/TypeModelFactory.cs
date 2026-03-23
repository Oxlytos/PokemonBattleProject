using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Pokemon.ContractDTOs.Interfaces;
using Pokemon.ContractDTOs.RequestModel;
using Pokemon.Infrastructure.Mapper;

namespace Pokemon.Infrastructure.Factories
{
    public class TypeModelFactory:ITypeModelFactory
    {
        public TypeModel Create(RequestTypeModel apiType)
        {
            //sCreate object
            TypeModel model = new TypeModel();


            //mappa it out
            model = TypeMapper.MapType(apiType);

            //return
            return model;
        }

     
    }
}
