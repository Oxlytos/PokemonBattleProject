using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Pokemon.ContractDTOs.RequestModel
{
    public class SpriteCollection
    {
        [JsonPropertyName("sprites")]
        public RequestSpriteModel SpriteModel { get; set; }
    }

    public class RequestSpriteModel
    {
        [JsonPropertyName("back_default")]
        public string BackDefault { get; set; }

        [JsonPropertyName("back_female")]
        public string? BackFemale { get; set; }

        [JsonPropertyName("back_shiny")]
        public string? BackShiny { get; set; }

        [JsonPropertyName("back_shiny_female")]
        public string? BackShinyFemale { get; set; }

        [JsonPropertyName("front_default")]
        public string FrontDefault { get; set; }

        [JsonPropertyName("front_female")]
        public string? FrontFemale { get; set; }

        [JsonPropertyName("front_shiny")]
        public string? FrontShiny { get; set; }

        [JsonPropertyName("front_shiny_female")]
        public string? FrontShinyFemale { get; set; }

        [JsonPropertyName("versions")] //fylla på med gen 1, alla spel där, gen 2, alla spel där osv
        public  GameVersions GameVersions { get; set; }
    }
    public class GameVersions
    {
        [JsonPropertyName("generation-iii")]
        public SpriteGenerationThree Gen3Sprites { get; set; }
    }
   public class SpriteGenerationThree
    {
        [JsonPropertyName("ruby-sapphire")]
        public RubySapphireSprites RubySapphireSprites { get; set; }
        [JsonPropertyName("emerald")]
        public EmeraldSprites EmeraldSprites { get; set; }
        [JsonPropertyName("firered-leafgreen")]
        public FireRedLeafGreenSprites FireRedLeafGreenSprites { get; set; }
    }
    public class FireRedLeafGreenSprites
    {
        [JsonPropertyName("front_default")]
        public string FrontDefault { get; set; }

        [JsonPropertyName("front_shiny")]
        public string FrontShiny { get; set; }

        [JsonPropertyName("back_default")]
        public string BackDefault { get; set; }

        [JsonPropertyName("back_shiny")]
        public string BackShiny { get; set; }
    }
    public class EmeraldSprites
    {
        [JsonPropertyName("front_default")]
        public string FrontDefault { get; set; }

        [JsonPropertyName("front_shiny")]
        public string FrontShiny { get; set; }

        [JsonPropertyName("back_default")]
        public string BackDefault { get; set; }

        [JsonPropertyName("back_shiny")]
        public string BackShiny { get; set; }
    }
    public class RubySapphireSprites
    {
        [JsonPropertyName("front_default")]
        public string FrontDefault { get; set; }

        [JsonPropertyName("front_shiny")]
        public string FrontShiny { get; set; }

        [JsonPropertyName("back_default")]
        public string BackDefault { get; set; }

        [JsonPropertyName("back_shiny")]
        public string BackShiny { get; set; }
    }
}
