using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Domain.Models.RequestModels;

namespace Pokemon.AppServices.Mappers
{
    public static class GeneralMapper
    {
        public static PartyPokemonModel MapBasic(RequestPokeonModel model)
        {
            var pokemon = new PartyPokemonModel();
            pokemon.Name = model.Name;
            pokemon.Level = 50;
            return pokemon;
        }
    }
}
