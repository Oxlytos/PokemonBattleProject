using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models.Models
{
    public class PokemonListRequestModel
    {
        [JsonPropertyName("results")]
        public PokemonModel[]? Result { get; set; }
    }
}
