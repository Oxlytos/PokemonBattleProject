using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Domain.Models.Game;
using Domain.Models.Models;

namespace Domain.Models.Base
{
    public class BasePokemon
    {
        public string? Name { get; set; }

        public string? Nickname { get; set; }

        public string? SpritePath { get; set; }
        public string? Url { get; set; }

        public StatModel[] Stats { get; set; }

        public TypeModel[] Types { get; set; }

        public MoveModel[] LearnedMoves { get; set; } = Array.Empty<MoveModel>();

        //public MoveRequestCollection Moves { get; set; }

        public SpriteModel Sprites { get; set; }

        public int? Health { get; set; }

        public int? Attack { get; set; }

        public int? Defense { get; set; }

        public int? SpecialAttack { get; set; }

        public int? SpecialDefense { get; set; }

        public int? Speed { get; set; }



       
    }
}
