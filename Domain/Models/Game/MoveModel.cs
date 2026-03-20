using System.Text.Json.Serialization;

namespace Domain.Models.Game
{
    public class MoveModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("pp")]
        public int? Pp { get; set; }

        [JsonPropertyName("power")]
        public float? Power { get; set; }

        [JsonPropertyName("accuracy")]
        public float? Accuracy { get; set; }

        [JsonPropertyName("priority")]
        public int? Priority { get; set; }

        [JsonPropertyName("generation")]
        public MoveGeneration MoveGeneration { get; set; }

        [JsonPropertyName("type")]
        public TypeModel Type { get; set; }
    }
    //May crit?
    //May leave foe paralyzed
    public class MoveGeneration
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
