using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;

namespace Pokemon.Infrastructure.Models
{
    public class TurnResult
    {
        public BattlePokemonModel PlayerCurrentPokemon { get; set; }
        public BattlePokemonModel AiCurrentPokemon { get; set; }
        public List<BattlePokemonModel> PlayerParty {  get; set; }
        public List<BattlePokemonModel> AiParty { get; set; }


    }
}
