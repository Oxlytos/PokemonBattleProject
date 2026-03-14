using System.Text.Json.Serialization;

namespace Domain.Models.Game
{
    public class StatModel
    {
        [JsonPropertyName("b_health")]
        public int BaseHealth { get; set; }

        [JsonPropertyName("b_attack")]
        public int BaseAttack { get; set; }

        [JsonPropertyName("b_defense")]
        public int BaseDefense { get; set; }

        [JsonPropertyName("b_sattack")]
        public int BaseSpecialAttack { get; set; }

        [JsonPropertyName("b_sdefense")]
        public int BaseSpecialDefense { get; set; }

        [JsonPropertyName("b_speed")]
        public int BaseSpeed { get; set; }
        /// <summary>
        /// /////////////
        /// </summary>

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
