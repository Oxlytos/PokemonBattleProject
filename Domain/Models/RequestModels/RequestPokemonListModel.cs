using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models.RequestModels
{
    public class RequestPokemonListModel
    {
        [JsonPropertyName("results")]
        public RequestPokeonModel[]? Result { get; set; }
    }
}
