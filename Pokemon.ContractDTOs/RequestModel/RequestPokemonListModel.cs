using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Pokemon.ContractDTOs.RequestModel
{
    public class RequestPokemonListModel
    {
        [JsonPropertyName("results")]
        public RequestPokeonModel[]? Result { get; set; }
    }
}
