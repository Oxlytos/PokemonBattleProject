using System.Text.Json.Serialization;

namespace Domain.Models.Game
{
    public class StatModel
    {
        //b_ för basestat
        //olika pokemon har base stat totals, som i spelen påverkas av IVS och EVs
        //Inte till denna iterationen av projektet, senare tillfälle kanske
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
        //Base stat * level / på något får vi aktiva health, attack osv för pokemon
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



        //Evs, specialisering av poäng upp till 528 eller något? Tag sen jag kollat på Evs i spelen
        //Slider till senare, om det är av intresse, blir lite mer competetive fokus då

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
