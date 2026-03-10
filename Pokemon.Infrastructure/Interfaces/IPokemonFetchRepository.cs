using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Models;

namespace Pokemon.Infrastructure.Interfaces
{
    public interface IPokemonFetchRepository
    {
        public Task<List<PokemonModel>> GetPokemonModelsAsync();
        public Task<PokemonModel> GetPokemonModelModelAsync(string name);
        public Task<TypeModel> GetTypeModelAsync(string name);
        public Task<MoveModel> GetMoveModelAsync(string name);
    }
}
