using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models.Models
{
    public class AbilityModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("effect_entries")]

        public EffectEntry[]? EffectEntries { get; set; }

        public string? Description { get; set; }

        public bool? IsHidden { get; set; }


    }
    public class EffectEntry
    {
        [JsonPropertyName("effect")]
        public string Effect {  get; set; }
        [JsonPropertyName("short_effect")]
        public string ShortEffectDescription { get; set; }

        [JsonPropertyName("language")]
        public Language Language { get; set; }

    }
    public class Language
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
      
    }
}
