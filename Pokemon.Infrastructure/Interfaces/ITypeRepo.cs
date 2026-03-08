using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Models;

namespace Pokemon.Infrastructure.Interfaces
{
    public interface ITypeRepo
    {
        public Task<string> GetTypeSprite(string name);
        public Task<TypeModel> GetTypeData(string name);

        public Task SaveTypeData(string name);
    }
}
