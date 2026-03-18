using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Base;
using Domain.Models.Game;
using Domain.Models.RequestModels;

namespace Pokemon.Services.Interfaces
{
    public interface IFetchService
    {
        //Alla pokemon
        public Task<List<RequestPokeonModel>> GetPokemonAsync();

        //Endast en
        public Task<RequestPokeonModel> GetPokemonSingularAsync(string name);
        //Type
        public Task<RequestTypeModel> GetTypeModelAsync(string name);

        //Move
        public Task<RequestMoveModel> GetMoveModelAsync(string name);

        //Absolut mest basic pokemon
        public Task<BasePokemon> GetBasePokemonAsync(string name);

        //Användbar movemodel
        public Task<MoveModel> GetSerialisedMoveModelAsync(string name);    

        //Användbar typ
        public Task<TypeModel> GetSerialisedTypeModelAsync(string name);
    }
}
