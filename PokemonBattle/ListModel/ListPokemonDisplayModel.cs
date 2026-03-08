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
    
        
        public ListPokemonDisplayModel(PokemonModel pokemon)
        {
            Pokemon = pokemon;
        }

        private string _types;
        public string Types
        {
            get
            {
                return _types;
            }
            set
            {
                _types = value;
                OnPropertyChanged(nameof(Types));
            }
        }

        private string _spritePath;
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
        private string[] _typePaths;
        public string[] SpriteTypePaths
        {
            get
            {
               return _typePaths;
            }
            set
            {
                _typePaths = value;
                OnPropertyChanged(nameof(SpriteTypePaths));
            }
        }

        void OnPropertyChanged(string propertyName)
        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
