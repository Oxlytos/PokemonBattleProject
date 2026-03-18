using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Domain.Models.RequestModels;
using Pokemon.AppServices.Interfaces;

namespace Pokemon.AppServices.Mappers
{
    public class GeneralMapper : IGeneralMapper
    {
        public PartyPokemonModel MapBasic(RequestPokeonModel model)
        {
            var pokemon = new PartyPokemonModel();
            pokemon.Name = model.Name;
            pokemon.Level = 50;
            return pokemon;
        }
    }
}
