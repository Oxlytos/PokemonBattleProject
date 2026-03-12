using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Base;
using Domain.Models.Game;
using Domain.Models.RequestModels;
using Pokemon.Infrastructure.Interfaces;
using Pokemon.Repository.Interfaces;
using Pokemon.Repository.Repositories;
using Pokemon.Services.Interfaces;

namespace Pokemon.Services.Services
{
    public class FetchService : IPokemonFetchService
    {
        private readonly IPokemonFetchRepository _repository;
        public FetchService(
            IPokemonFetchRepository pokemonFetchRepository
            )
        {
            _repository = pokemonFetchRepository;
        }

        public async Task<BasePokemon> GetBasePokemonAsync(string name)
        {
            return await _repository.GetBasePokemonAsync(name);
        }

        public async Task<RequestMoveModel> GetMoveModelAsync(string name)
        {
            return await _repository.GetMoveModelAsync(name);
        }

        public async Task<List<RequestPokeonModel>> GetPokemonAsync()
        {
            return await _repository.GetPokemonModelsAsync();
        }

        public async Task<RequestPokeonModel> GetPokemonSingularAsync(string name)
        {
            return await _repository.GetPokemonModelModelAsync(name);
        }

        public async Task<MoveModel> GetSerialisedMoveModelAsync(string name)
        {
            return await _repository.GetSerialisedMoveModelAsync(name);
        }

        public async Task<TypeModel> GetSerialisedTypeModelAsync(string name)
        {
            return await _repository.GetSerialisedTypeModelAsync(name);
        }

        public async Task<RequestTypeModel> GetTypeModelAsync(string name)
        {
            return await _repository.GetTypeModelAsync(name);
        }
    }
}
