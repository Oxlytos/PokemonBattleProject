using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;

namespace Pokemon.Infrastructure.Models
{
    public class TurnResult
    {
        public List<string> BattleActionMessages { get; set; } = new List<string>();
        public BattlePokemonModel PlayerCurrentPokemon { get; set; }
        public BattlePokemonModel AiCurrentPokemon { get; set; }
        public List<BattlePokemonModel> PlayerParty {  get; set; }
        public List<BattlePokemonModel> AiParty { get; set; }

        public bool PlayerFainted { get; set; } = false;
        public bool AiFainted { get;set; } = false;

    }
}
