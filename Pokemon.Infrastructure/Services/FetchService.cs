using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Models;
using Pokemon.Infrastructure.Interfaces;
using Pokemon.Repository.Interfaces;
using Pokemon.Repository.Repositories;
using Pokemon.Services.Interfaces;

namespace Pokemon.Services.Services
{
    public class FetchService : IPokemonFetchService
    {
        private readonly IPokemonFetchRepository _repository;
        private readonly IJsonStorage _storage;
        public FetchService(
            IPokemonFetchRepository pokemonFetchRepository,
            IJsonStorage jsonStorage
            )
        {
            _repository = pokemonFetchRepository;
            _storage = jsonStorage;
        }

        public async Task<MoveModel> GetMoveModelAsync(string name)
        {
            return await _repository.GetMoveModelAsync(name);
        }

        public async Task<List<PokemonModel>> GetPokemonAsync()
        {
            return await _repository.GetPokemonModelsAsync();
        }

        public async Task<PokemonModel> GetPokemonSingularAsync(string name)
        {
            return await _repository.GetPokemonModelModelAsync(name);
        }

        public async Task<TypeModel> GetTypeModelAsync(string name)
        {
            return await _repository.GetTypeModelAsync(name);
        }
    }
}
