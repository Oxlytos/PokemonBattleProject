using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Domain.Models.Models;

namespace Domain.Models
{
    public class MoveRequestCollection
    {
        [JsonPropertyName("moves")]
        public MoveRequestModel[] Moves { get; set; }
    }
    public class MoveRequestModel
    {
        [JsonPropertyName("move")]
        public Move Move {  get; set; }

        public int? Power { get; set; }

        public string? TypeName { get; set; }

        public string DisplayInfo => $"{Move.Name} | {TypeName} | {Power}";
        
    }
    public class Move
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
   
}
