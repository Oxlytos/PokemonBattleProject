using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Models;
using Pokemon.Infrastructure.Interfaces;
using Pokemon.Services.Interfaces;

namespace Pokemon.Services.Services
{
    public class PokemonFetchService : IPokemonFetchService
    {
        private readonly IPokemonFetchRepository _repository;
        public PokemonFetchService(IPokemonFetchRepository pokemonFetchRepository)
        {
            _repository = pokemonFetchRepository;
        }
        public async Task<List<PokemonModel>> GetPokemonAsync()
        {
            return await _repository.GetPokemonModelsAsync();
        }

        public async Task<PokemonModel> GetPokemonSingularAsync(string name)
        {
            return await _repository.GetPokemonModelModelAsync(name);
        }
    }
}
