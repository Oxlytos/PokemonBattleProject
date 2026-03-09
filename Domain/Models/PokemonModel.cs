using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
namespace Domain.Models.Models
{
    public class PokemonModel
    {
        // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
        //Basic request for a pokemon dosen't include things like all move data, types, descriptors etc
        //But a resource link through name and url
        //Retrieve abilities by creating a APIResourceConnection
        //Name = "Overgrow" Url = "//"
        [JsonPropertyName("id")]
        public int PokedexId { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("stats")]
        public PokemonStat[] Stats { get; set; }

        [JsonPropertyName("types")]
        public TypeRequest[] Types { get; set; }

        public MoveRequestCollection Moves { get; set; }

        public SpriteCollection Sprites { get; set; }

        public int? Health { get; set; }

        public int? Attack {  get; set; }

        public int? Defense { get; set; }

        public int? SpecialAttack { get; set; }

        public int? SpecialDefense { get; set; }

        public int? Speed { get; set; }

        public AbilityModel Ability { get; set; }

        public List<TypeModel> Type { get; set; }

       
        public string SpritePath { get; set; }

        public async void SetBaseStatTotals()
        {
            //Värderna
            var healthTask = SetHealthBaseStat();
            var attackTask = SetAttackBaseStat();
            var defenseTask = SetDefenseBaseStat();
            var spAttackTask = SetSpecialAttackBaseStat();
            var spDefenseTask = SetSpecialDefenseBaseStat();
            var speedTask = SetSpeedBaseStat();

            //Assigna typ som en queue
            await Task.WhenAll(healthTask,attackTask,defenseTask,spAttackTask,spDefenseTask,speedTask);

            Health = await healthTask;
            Attack = await attackTask;
            Defense = await defenseTask;
            SpecialAttack = await spAttackTask;
            SpecialDefense = await spDefenseTask;
            Speed = await speedTask;
        }
       
        SpriteModel GetSprite()
        {
            return null;
        }
        async Task<int?> SetHealthBaseStat()
        {
            return Stats[0].BaseStat;
        }
        private async Task<int?> SetSpeedBaseStat()
        {
            return Stats[5].BaseStat;
        }

        private async Task<int?> SetSpecialDefenseBaseStat()
        {
            return Stats[4].BaseStat;
        }

        private async Task<int?> SetSpecialAttackBaseStat()
        {
            return Stats[3].BaseStat;
        }

        private async Task<int?> SetDefenseBaseStat()
        {
            return Stats[2].BaseStat;
        }

        private async Task<int?> SetAttackBaseStat()
        {
            return Stats[1].BaseStat;
        }

        public class APIResourceConnection<TDetails>
        {
            [JsonPropertyName("name")]
            public string? Name { get; set; }

            [JsonPropertyName("url")]
            public string? Url { get; set; }
        }
       
        public class AbilityDetails()
        {

        }
        public class MoveDetails()
        {

        }
        public class TypeDetails()
        {

        }
        public class StatDetails()
        {

        }
       
        public class PokemonAbility
        {
            [JsonPropertyName("ability")]
            public APIResourceConnection<AbilityDetails> Ability { get; set; }

            [JsonPropertyName("is_hidden")]
            public bool IsHidden { get; set; }

            [JsonPropertyName("slot")]
            public int Slot { get; set; }
        }

        public class Ability2
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("url")]
            public string Url { get; set; }
        }


        public class Item
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("url")]
            public string Url { get; set; }
        }

        public class PokemonMove
        {
            [JsonPropertyName("move")]
            public APIResourceConnection<MoveDetails> Move { get; set; }

            public MoveDetails? MoveDetails { get; set; }

        }
       
        public class Move2
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("url")]
            public string Url { get; set; }
        }
        public class PokemonStat
        {
            [JsonPropertyName("base_stat")]
            public int BaseStat { get; set; }

            [JsonPropertyName("effort")]
            public int Effort { get; set; }

            [JsonPropertyName("stat")]
            public APIResourceConnection<StatDetails> Stat { get; set; }
        }

        public class PokemonType
        {
            [JsonPropertyName("slot")]
            public int Slot { get; set; }

            [JsonPropertyName("type")]
            public APIResourceConnection<TypeDetails> Type { get; set; }
        }


    }


}
