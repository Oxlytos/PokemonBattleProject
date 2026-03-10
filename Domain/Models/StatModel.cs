using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class StatModel  
    {
        [JsonPropertyName("health")]
        public int Health { get; set; }

        [JsonPropertyName("attack")]
        public int Attack { get; set; }

        [JsonPropertyName("defense")]
        public int Defense { get; set; }

        [JsonPropertyName("sattack")]
        public int SpecialAttack { get; set; }

        [JsonPropertyName("sdefense")]
        public int SpecialDefense { get; set; }

        [JsonPropertyName("speed")]
        public int Speed { get; set; }


        [JsonPropertyName("health_evs")]
        public int? HealthEvs;

        [JsonPropertyName("attack_evs")]
        public int? AttackEvs;

        [JsonPropertyName("defense_evs")]
        public int? DefenseEvs;

        [JsonPropertyName("sattack_evs")]
        public int? SpecialAttackEvs;

        [JsonPropertyName("sdefense_evs")]
        public int? SpecialDefenseEvs;

        [JsonPropertyName("speed_evs")]
        public int? SpeedEvs;

    }
}
