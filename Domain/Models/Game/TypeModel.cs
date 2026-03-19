using System.Text.Json.Serialization;

namespace Domain.Models.Game
{
    public class TypeModel
    {
        //Spelets egna kunskap om alla typer
        //Vet inget om omvärlden
        public string Name { get; set; }

        public bool IsSpecialDamage { get; set; }

        [JsonPropertyName("effective_against")]
        public List<string>? Effectivnesses { get; set; } = new List<string>();

        [JsonPropertyName("this_type_immune_to")]
        public List<string>? Immunities { get; set; } = new List<string>();

        [JsonPropertyName("other_types_immune_to_this")]
        public List<string>? TypesImmune { get; set; } = new List<string>();

        [JsonPropertyName("weak_to")]
        public List<string>? Weaknesses { get; set; } = new List<string>();

        [JsonPropertyName("resists_this")]
        public List<string>? Resistances { get; set; } = new List<string>();

        [JsonPropertyName("weak_against")]
        public List<string>? TypesResisting { get; set; } = new List<string>();
    }
}
