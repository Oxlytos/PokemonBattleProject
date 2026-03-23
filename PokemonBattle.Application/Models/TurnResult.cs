using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;

namespace Pokemon.AppServices.Models
{
    public class TurnResult
    {

        //Log battle events and actions
        //"It super effective!"
        //"Charizard used Flamethrower!"
        //"Sandlash fainted!"
        public List<string> BattleActionMessages { get; set; } = new List<string>();

        //Mest UI info för viewmodel
        public BattlePokemonModel PlayerCurrentPokemon { get; set; }
        public BattlePokemonModel AiCurrentPokemon { get; set; }
        public List<BattlePokemonModel> PlayerParty {  get; set; }
        public List<BattlePokemonModel> AiParty { get; set; }

        //When fainted
        public bool PlayerFainted { get; set; } = false;
        public bool AiFainted { get;set; } = false;

        //On win
        public bool PlayerWin {  get; set; } = false;
        public bool AiWin { get; set;} = false;

    }
}
