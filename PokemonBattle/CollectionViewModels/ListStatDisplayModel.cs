using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;

namespace PokemonBattle.CollectionViewModels
{
    public class ListStatDisplayModel
    {
        //format is "Health: XX"
        public string DisplayHealth { get; set; }
        public string DisplayAttack { get; set; }
        public string DisplayDefense { get; set; }
        public string DisplaySpecialAttack { get; set; }
        public string DisplaySpecialDefense { get; set; }
        public string DisplaySpeed { get; set; }
    }
}
