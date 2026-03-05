using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Models;
using Pokemon.Services.Interfaces;

namespace Pokemon.Services.Services
{
    public class PokemonFetchService : IPokemonFetchService
    {
        public Task<List<PokemonModel>> GetPokemonAsync()
        {
            throw new NotImplementedException();
        }

        public Task<PokemonModel> GetPokemonSingularAsync(string name)
        {
            throw new NotImplementedException();
        }
    }
}
