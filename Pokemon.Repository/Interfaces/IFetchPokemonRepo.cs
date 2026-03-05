using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Models;

namespace Pokemon.Repository.Interfaces
{
    public interface IFetchPokemonRepo
    {
        public Task<List<PokemonModel>> GetPokemon();
        public Task<PokemonModel> GetPokemonSingular(string name);
    }
}
