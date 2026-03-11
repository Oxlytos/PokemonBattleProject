using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Domain.Models.RequestModels;

namespace Pokemon.Services.Interfaces
{
    public interface IPokemonFetchService
    {
        //Alla pokemon
        public Task<List<RequestPokeonModel>> GetPokemonAsync();

        //Endast en
        public Task<RequestPokeonModel> GetPokemonSingularAsync(string name);

        public Task<RequestTypeModel> GetTypeModelAsync(string name);

        public Task<MoveModel> GetMoveModelAsync(string name);
    }
}
