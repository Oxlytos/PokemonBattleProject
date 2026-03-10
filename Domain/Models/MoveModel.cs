using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models.Models
{
    public class MoveModel
    {
       
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("pp")]
        public int? Pp {  get; set; }

        [JsonPropertyName("power")]
        public float? Power { get; set; }

        [JsonPropertyName("accuracy")]
        public float? Accuracy { get; set; }

        [JsonPropertyName("priority")]
        public int? Priority { get; set; }

        [JsonPropertyName("generation")]
        public MoveGeneration MoveGeneration { get; set; }

        [JsonPropertyName("type")]
        public TypeRequest Type { get; set; }


        public List<MoveType>? MoveType { get; set; }

        public List<EffectChange>? MoveEffects { get; set; }

        //För på sig själv som Bulk Up, eller mot fiende som Superpower
        public List<TargetModel> TargetOfMove { get; set; }
    }
    //May crit?
    //May leave foe paralyzed
    public class MoveGeneration
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }


    }
    public class EffectChange
    {
        public int EffectChance { get; set; }
    }

    //For physical, special
    //Buff, debuff
    //Other
    public class MoveType
    {

    }
    public enum MoveTypes
    {
        Physical, //Earthquake
        Special, //Flamethrower
        DecreaseOwnStat, //Shellsmash
        IncreaseOwnStat, //Shellsmash again
        DecreaseOtherStat, //Growl
        IncreaseOtherStat, //Flatter
        OneHitKO, //Fissure
        Status, //Thunderwave, wish, yawn

    }
}
