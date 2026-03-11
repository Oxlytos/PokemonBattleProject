using System.Text.Json.Serialization;

namespace Domain.Models.Game
{
    public class TypeModel
    {
        //Spelets egna kunskap om alla typer
        //Vet inget om omvärlden
        public string Name { get; set; }

        [JsonPropertyName("effective_against")]
        public List<TypeModel>? Effectivnesses { get; set; }

        [JsonPropertyName("this_type_immune_to")]
        public List<TypeModel>? Immunities { get; set; }

        [JsonPropertyName("other_types_immune_to_this")]
        public List<TypeModel>? TypesImmune { get; set; }

        [JsonPropertyName("weak_to")]
        public List<TypeModel>? Weaknesses { get; set; }

        [JsonPropertyName("resists_this")]
        public List<TypeModel>? Resistances { get; set; }

        [JsonPropertyName("weak_against")]
        public List<TypeModel>? TypesResisting { get; set; }
    }
}
