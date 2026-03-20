using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Base;
using Domain.Models.Game;
using Pokemon.ContractDTOs.Interfaces;
using Pokemon.ContractDTOs.RequestModel;
using Pokemon.Infrastructure.Interfaces;

namespace Pokemon.Infrastructure.Services
{
    public class FetchService : IFetchService
    {
        private readonly IFetchRepository _repository;
        public FetchService(
            IFetchRepository pokemonFetchRepository
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
            return await _repository.GetPokemonModel(name);
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
