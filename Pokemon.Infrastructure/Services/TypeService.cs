using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Models;
using Pokemon.Infrastructure.Interfaces;

namespace Pokemon.Infrastructure.Services
{
    public class TypeService : ITypeService
    {
        private readonly ITypeRepo _repo;
        public TypeService(ITypeRepo typeRepo)
        {
            _repo = typeRepo;
        }
        public async Task<TypeModel> GetTypeData(string spriteName)
        {
            return await _repo.GetTypeData(spriteName);
        }

        public async Task<string> GetTypeSprite(string spriteName)
        {
            return await _repo.GetTypeSprite(spriteName);
        }

        public Task SaveTypeData(string typeName)
        {
            throw new NotImplementedException();
        }
    }
}
