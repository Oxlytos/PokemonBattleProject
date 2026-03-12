using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using PokemonBattle.ListModel;

namespace PokemonBattle.Mapper
{
    public static class BasicListPokemoMapper
    {
        public static ListPokemonDisplayModel MapBasic(PartyPokemonModel model)
        {
            var display = new ListPokemonDisplayModel(model);
            display.Types = model.Types.ToArray();
            return display;
        }
    }
}
