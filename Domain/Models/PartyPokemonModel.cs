using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Domain.Models.Models;

namespace Domain.Models
{
    public class PartyPokemonModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("nickname")]
        public string? NickName { get; set; }

        [JsonPropertyName("moves")]
        public List<MoveModel> Moves { get; set; }

        [JsonPropertyName("types")]
        public List<TypeModel> Types { get; set; }

        [JsonPropertyName("stats")]
        public StatModel Stats { get; set; }

        [JsonPropertyName("level")]
        public int Level { get; set; } = 50;


    }
}
