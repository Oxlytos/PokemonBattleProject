using System.Text.Json.Serialization;

namespace Domain.Models.Game
{
    public class PartyPokemonModel
    {
        public PartyPokemonModel()
        {
            
        }
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("nickname")]
        public string? Nickname { get; set; }

        [JsonPropertyName("moves")]
        public List<MoveModel> Moves { get; set; } = new List<MoveModel>();

        [JsonPropertyName("types")]
        public List<string> Types { get; set; }

        [JsonPropertyName("stats")]
        public StatModel Stats { get; set; }

        [JsonPropertyName("level")]
        public int Level { get; set; } = 50;


    }
}
