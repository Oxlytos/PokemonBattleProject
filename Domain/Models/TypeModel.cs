using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models.Models
{
  
    public class TypeRequest
    {
        [JsonPropertyName("slot")]
        public int SlotNumber { get; set; }

        [JsonPropertyName("type")]
        public TypeModel Types { get; set; }
    }
    public class TypeData
    {
        [JsonPropertyName("name")]
        public string TypeName { get; set; }
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
    public class TypeModel
    {

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("damage_relations")]
        public DamageRelations DamageRelations { get; set; }

        //Stark emot => Gräs mot vatten
        
    }
}
