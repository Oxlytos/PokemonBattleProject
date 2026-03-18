using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Domain.Models.RequestModels;

namespace Domain.Interface
{
    public interface ITypeModelFactory
    {
        public TypeModel Create(RequestTypeModel apiType);
    }
}
