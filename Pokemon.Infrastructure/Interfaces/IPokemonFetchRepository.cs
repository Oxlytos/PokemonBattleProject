using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Base;
using Domain.Models.Game;
using Domain.Models.RequestModels;

namespace Pokemon.Infrastructure.Interfaces
{
    public interface IPokemonFetchRepository
    {
        public Task<List<RequestPokeonModel>> GetPokemonModelsAsync();
        public Task<RequestPokeonModel> GetPokemonModelModelAsync(string name);
        public Task<RequestTypeModel> GetTypeModelAsync(string name);
        public Task<RequestMoveModel> GetMoveModelAsync(string name);
        public Task<BasePokemon> GetBasePokemonAsync(string name);
        public Task<MoveModel> GetSerialisedMoveModelAsync(string name);
        public Task<TypeModel> GetSerialisedTypeModelAsync(string name);
    }
}
