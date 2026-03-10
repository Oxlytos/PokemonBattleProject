using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Models;

namespace Pokemon.Services.Interfaces
{
    public interface IPokemonFetchService
    {
        //Alla pokemon
        public Task<List<PokemonModel>> GetPokemonAsync();

        //Endast en
        public Task<PokemonModel> GetPokemonSingularAsync(string name);

        public Task<TypeModel> GetTypeModelAsync(string name);

        public Task<MoveModel> GetMoveModelAsync(string name);
    }
}
