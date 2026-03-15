using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;

namespace Pokemon.Infrastructure.Models
{
    public class AiTeam
    {
        public string? Name { get; set; }
        public List<PartyPokemonModel> AiPokemon { get; set; } = new List<PartyPokemonModel>();
    }
}
