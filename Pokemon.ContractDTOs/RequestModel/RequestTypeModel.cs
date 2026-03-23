using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Pokemon.ContractDTOs.RequestModel
{
  
    public class TypeRequest
    {
        [JsonPropertyName("slot")]
        public int SlotNumber { get; set; }

        [JsonPropertyName("type")]
        public RequestTypeModel Types { get; set; }

      
    }
    public class OldTypes
    {
        [JsonPropertyName("generation")]
        public GenerationInfo? GenInfo;
        public string? Generation {  get; set; }

        [JsonPropertyName("types")]
        public TypeRequest[]? OldTypesInfo { get; set; }
        
    }
    public class GenerationInfo
    {
        [JsonPropertyName("name")]
        public string? Gen { get; set; }

    }
    public class TypeData
    {
        [JsonPropertyName("name")]
        public string TypeName { get; set; }

        [JsonPropertyName("url")]
        public string TypeUrl { get; set; }
    }
    public class DamageRelations
    {
        [JsonPropertyName("double_damage_to")]
        public TypeData[] Effectivnesses { get; set; }

        //Immunities => Denna typ (t.ex. flying) är immun mot t.ex. ground 
        [JsonPropertyName("no_damage_from")]
        public TypeData[] Immunities { get; set; }

        //Steel är t.ex. immun mot denna typ som kan vara poison
        [JsonPropertyName("no_damage_to")]
        public TypeData[] TypesImmune { get; set; }

        [JsonPropertyName("double_damage_from")]
        public TypeData[] Weaknesses { get; set; }

        [JsonPropertyName("half_damage_from")]
        public TypeData[] Resistances { get; set; }

        [JsonPropertyName("half_damage_to")]
        public TypeData[] TypesResisting {  get; set; }

    }
    public class RequestTypeModel
    {

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("damage_relations")]
        public DamageRelations DamageRelations { get; set; }

        [JsonPropertyName("move_damage_class")]
        public DamageClass DamageTypeClass { get; set; }

        [JsonPropertyName("sprites")]
        public TypeSpriteCollection Sprites { get; set; }

        //Stark emot => Gräs mot vatten
        
    }
    public class DamageClass
    {
        [JsonPropertyName("name")]
        public string DamageType { get; set; }

    }
    public class TypeSpriteCollection
    {
        [JsonPropertyName("generation-iii")]
        public GenerationThreeTypeSprites TypeCollections { get; set; }
    }
    public class GenerationThreeTypeSprites
    {
        [JsonPropertyName("firered-leafgreen")]
        public TypeIconSprite FireRedLeafGreenTypeIconSprite { get; set; }
    }
    public class TypeIconSprite
    {
        [JsonPropertyName("name_icon")]
        public string TypeIconUrl { get; set; }
    }
}
