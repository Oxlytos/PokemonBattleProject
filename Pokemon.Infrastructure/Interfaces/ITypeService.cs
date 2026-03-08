using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Models;

namespace Pokemon.Infrastructure.Interfaces
{
    public interface ITypeService
    {
        public Task<string> GetTypeSprite(string spriteName);
        public Task<TypeModel> GetTypeData(string spriteName);
        public Task SaveTypeData(string typeName);
    }
}
