using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;

namespace Domain.Interface
{
    public interface ITypeDataRepo
    {
        TypeModel GetTypeModel(string name);
    }
}
