using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Pokemon.ContractDTOs.RequestModel;

namespace Pokemon.ContractDTOs.Interfaces
{
    public interface ITypeModelFactory
    {
        public TypeModel Create(RequestTypeModel apiType);
    }
}
