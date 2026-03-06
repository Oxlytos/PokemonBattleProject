using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Models;

namespace PokemonBattle.ListModel
{
    public class ListPokemonDisplayModel : INotifyPropertyChanged
    {
        //Dummare model bara för listviews, ska inte ha någon annan data associerad, förutom sprite o namn
        //När ändras, uppdatera
        public event PropertyChangedEventHandler? PropertyChanged;

        //Orginella pokemonen
        public PokemonModel Pokemon { get; }
        private string _spritePath;
        
        public ListPokemonDisplayModel(PokemonModel pokemon)
        {
            Pokemon = pokemon;
        }
        public string SpritePath
        {
            get { return _spritePath; }
            set
            {
                if(_spritePath != value)
                {
                    _spritePath = value;
                    OnPropertyChanged(nameof(SpritePath));
                }
            }
        }
        public string Name => Pokemon.Name;


        void OnPropertyChanged(string propertyName)
        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
