using System.Text.Json.Serialization;

namespace Domain.Models.Game
{
    public class PartyPokemonModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
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
