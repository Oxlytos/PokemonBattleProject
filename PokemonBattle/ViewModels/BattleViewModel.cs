using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Domain.Models.Game;
using Domain.Models.RequestModels;
using Pokemon.AppServices.Factories;
using Pokemon.Infrastructure.Interfaces;
using Pokemon.Infrastructure.Services;
using PokemonBattle.Facades;
using PokemonBattle.ListModel;

namespace PokemonBattle.ViewModels
{
    public class BattleViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private IBattleService _battleService;


        private UIFacade _uiFacade;
        private readonly ListPokemonDisplayModelFactory _displayModelFactory;
        public ObservableCollection<RequestPokeonModel> AllPokemon { get; }
        public ObservableCollection<ListPokemonDisplayModel> DisplayPlayerParty { get; } = new();

        private ObservableCollection<string> _currentPlayerPokemonMoves;
        public ObservableCollection<string> CurrentPlayerPokemonMoves
        {
            get => _currentPlayerPokemonMoves;
            set
            {
                _currentPlayerPokemonMoves = value;
                OnPropertyChanged(nameof(CurrentPlayerPokemonMoves));
            }
        }
        private ListPokemonDisplayModel _playerChoosenPokemon;
        public ListPokemonDisplayModel PlayerChoosenPokemon
        {
            get {  return _playerChoosenPokemon; }
            set
            {
                if(_playerChoosenPokemon != value)
                {
                    Console.WriteLine(PlayerChoosenPokemon.PartyPoke.Level);
                    _playerChoosenPokemon = value;
                    OnPropertyChanged(nameof(PlayerChoosenPokemon));
                }
            }
        }
        private string _currentPlayerNickname;
        public string CurrentPlayerNickname
        {
            get { return _currentPlayerNickname; }
            set
            {
                if (_currentPlayerNickname != value)
                {
                    _currentPlayerNickname = value;
                    OnPropertyChanged(nameof(CurrentPlayerNickname));
                }
            }
        }
        private ImageSource _playerPokemonImage;
        public ImageSource PlayerPokemonImage
        {
            get { return _playerPokemonImage; }
            set
            {
                if(_playerPokemonImage != value)
                {
                    _playerPokemonImage = value;
                    OnPropertyChanged(nameof(PlayerPokemonImage));
                }
            }
        }

        private ImageSource _opponentPokemon;
        public ImageSource OpponentPokemon
        {
            get { return _opponentPokemon; }
            set
            {
                if( _opponentPokemon != value)
                {
                    _opponentPokemon = value;
                    OnPropertyChanged(nameof(OpponentPokemon));
                }
            }
        }
        private PartyPokemonModel _currentPlayerPartyPokemon;
        public PartyPokemonModel CurrentPlayerPartyPokemon
        {
            get { return _currentPlayerPartyPokemon; }
            set
            {
                if ( _currentPlayerPartyPokemon != value)
                {
                    _currentPlayerPartyPokemon = value;
                    OnPropertyChanged(nameof(CurrentPlayerPartyPokemon));
                }
            }
        }
        public ICommand ClickMoveCommand { get; }
        public ICommand PlayerAction { get; }

        public ICommand OpponentAction { get; }
        public ICommand IncreaseTurn { get; }
        public ICommand InflictDamage { get; }
        public ICommand TakeDamage { get; }
        public class MoveItem
        {
            public string Name { get; set; }
            public ICommand ClickMoveCommand { get; set; }
        }
        public BattleViewModel(UIFacade uIFacade, ListPokemonDisplayModelFactory displayModelFactory, IBattleService battleService)
        {
            _uiFacade = uIFacade;
            _battleService = battleService;
            _displayModelFactory = displayModelFactory;
            _=RebuildTeamDisplay();
            _ = LoadGraphics();
            ClickMoveCommand = new Command<string>(async (moveName) => await OnClickMoveCommand(moveName));
            //DisplayTeamPokemon = new ObservableCollection<ListPokemonDisplayModel> _uiFacade.GetPokemonTeamAsync();
        }

        //Härifrån, battleservice/Facade för att hantera skada fiende
        //Sen AI
        //Sen mer grafik
        //Färdig?
        //Crazy
        private async Task OnClickMoveCommand(string moveName)
        {
            Console.WriteLine($"Clicked move {moveName}");
            throw new NotImplementedException();
        }

        public async Task RebuildTeamDisplay()
        {
            DisplayPlayerParty.Clear();
            await _battleService.LoadPartyPokemon();
            CurrentPlayerPartyPokemon = await _battleService.GetFirstPartyPokemon();
            var curMoves = CurrentPlayerPartyPokemon.Moves.Select(e => e.Name);

            CurrentPlayerPokemonMoves = new ObservableCollection<string>(curMoves);
            Console.WriteLine(CurrentPlayerPokemonMoves);

            var pokemon = await _uiFacade.GetPokemonTeamAsync();
            foreach (var pokemin in pokemon)
            {
                var display = await _displayModelFactory.CreateBackFacingSprite(pokemin);
                OnPropertyChanged(nameof(Pokemon));
                DisplayPlayerParty.Add(display);
            }
            var imageSource = DisplayPlayerParty.FirstOrDefault();


            PlayerPokemonImage = ImageSource.FromFile(imageSource.SpritePath);
            List<string> opponent = new List<string>();
            foreach(var pokemin in pokemon)
            {
                var display = await _displayModelFactory.CreateFrontFacingSprite(pokemin);
                opponent.Add(display.SpritePath);
            }
            Console.WriteLine(PlayerPokemonImage);
            OpponentPokemon = ImageSource.FromFile(opponent.FirstOrDefault());
        }
        public async Task LoadGraphics()
        {

        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
