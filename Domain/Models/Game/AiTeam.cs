using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Game
{
    public class AiTeam
    {
        public string? Name { get; set; }
        public List<PartyPokemonModel> AiPokemon { get; set; } = new List<PartyPokemonModel>();
    }
}
