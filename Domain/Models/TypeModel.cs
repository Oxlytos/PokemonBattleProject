using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models.Models
{
    public class TypeModel
    {
        public string Name { get; set; }

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
        public List<Type>? Immunities { get; set; }
    }
}
