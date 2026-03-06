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
    public class TypeModel
    {

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        //Stark emot => Gräs mot vatten
        [JsonPropertyName("double_damage_to")]
        public List<Type>? Effectivness { get; set; }

        //Svag emot => Gräs tål inte eld
        [JsonPropertyName("double_damage_from")]
        public List<Type>? Weaknesses { get; set; }

        //Typ resistance => Gräs tål vatten
        [JsonPropertyName("half_damage_from")]
        public List<Type>? TypeResistances { get; set; }

        //Typ resiting => Steel resistar gräs
        [JsonPropertyName("half_damage_to")]
        public List<Type>? TypesResisting {  get; set; }

        //Immunities => Denna typ (t.ex. flying) är immun mot t.ex. ground 
        [JsonPropertyName("no_damage_from")]
        public List<Type>? Immunities { get; set; }

        //Steel är t.ex. immun mot denna typ som kan vara poison
        [JsonPropertyName("no_damage_to")]
        public List<Type>? TypesImmune { get; set; }
    }
}
