using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Domain.Models.RequestModels;

namespace PokemonBattle.ListModel
{
    public class ListPokemonDisplayModel : INotifyPropertyChanged
    {
        //Dummare model bara för listviews, ska inte ha någon annan data associerad, förutom sprite o namn
        //När ändras, uppdatera
        public event PropertyChangedEventHandler? PropertyChanged;
        public ListPokemonDisplayModel(PartyPokemonModel partyPokemonModel)
        {
            PartyPoke = partyPokemonModel;
        }

        //Orginella pokemonen
        public PartyPokemonModel PartyPoke { get; set; }

        private string[] _types;
        public string[] Types
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
        //länkar till själva bild på hårdisk
        private ImageSource _spriteImage;
        public ImageSource SpriteImage
        {
            get => _spriteImage;
            set
            {
                if (_spriteImage != value)
                {
                    _spriteImage = value;
                    OnPropertyChanged(nameof(SpriteImage));
                }
            }
        }
        //vägen dit till bilden
        private string _spritePath;
        public string SpritePath
        {
            get { return _spritePath; }
            set
            {
                if(_spritePath != value)
                {
                    _spritePath = value;
                    //from file hjälper och ta bort //User//Files till relativ väg från vad jag förstår
                    SpriteImage = ImageSource.FromFile(_spritePath);
                    Console.WriteLine(SpriteImage);
                    OnPropertyChanged(nameof(SpritePath));
                }
            }
        }
        public string Name => PartyPoke.Name;
        public string? Nickname
        {
            get=> PartyPoke.Nickname;
            set
            {
                if(PartyPoke.Nickname != value)
                {
                    PartyPoke.Nickname = value;
                    OnPropertyChanged(nameof(Nickname));
                }
            }
        }
        private ListMoveDisplayModel[] _displayMoves;
        public ListMoveDisplayModel[] DisplayMoves
        {
            get
            {
                return _displayMoves;
            }
            set
            {
                _displayMoves = value;
                OnPropertyChanged(nameof(DisplayMoves));
            }
        }
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
