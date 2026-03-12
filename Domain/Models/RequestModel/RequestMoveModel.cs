using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Domain.Models.Models;

namespace Domain.Models.RequestModels
{
    public class MoveRequestCollection
    {
        [JsonPropertyName("moves")]
        public RequestMoveModel[] Moves { get; set; }
    }
    public class RequestMoveModel
    {
        [JsonPropertyName("move")]
        public Move Move {  get; set; }

        [JsonPropertyName("power")]
        public int? Power { get; set; }

        [JsonPropertyName("type")]
        public MoveType MoveTypeInfo { get; set; }

        public string? TypeName { get; set; }

        public string DisplayInfo => $"{Move.Name} | {TypeName} | {Power}";
        
    }
    public class MoveType
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("url")]
        public string? Url { get; set; }

    }
    public class Move
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
   
}
