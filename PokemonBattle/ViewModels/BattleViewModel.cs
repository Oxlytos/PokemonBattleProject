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
using Domain.Services;
using Pokemon.AppServices.Factories;
using Pokemon.Infrastructure.Interfaces;
using Pokemon.Infrastructure.Models;
using Pokemon.Infrastructure.Services;
using PokemonBattle.Facades;
using PokemonBattle.ListModel;

namespace PokemonBattle.ViewModels
{
    public class BattleViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private IBattleService _battleService;
        private TypeDataService _typeDataService;
        private UIFacade _uiFacade;
        private BattleFacade _battleFacade;
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
        private BattlePokemonModel _currentPlayerPartyPokemon;
        public BattlePokemonModel CurrentPlayerPartyPokemon
        {
            get { return _currentPlayerPartyPokemon; }
            set
            {
                if ( _currentPlayerPartyPokemon != value)
                {
                    _currentPlayerPartyPokemon = value;
                    OnPropertyChanged(nameof(CurrentPlayerPartyPokemon));
                   _= GetPokemonImage(_currentPlayerPartyPokemon);
                }
            }
        }
        private BattlePokemonModel _currentAiPokemon;
        public BattlePokemonModel CurrentAiPokemon
        {
            get { return _currentAiPokemon; }
            set
            {
                if ( _currentAiPokemon != value)
                {
                    _currentAiPokemon = value;
                    OnPropertyChanged(nameof(CurrentAiPokemon));
                    _=GetOpponentPokemonImage(_currentAiPokemon);
                }
            }
        }
        private async Task GetOpponentPokemonImage(BattlePokemonModel pokemon)
        {
            OpponentPokemon=  await _uiFacade.LoadPokemonFrontSpritePathAsync(pokemon.PartyPokemon.Name);
        }

        private async Task GetPokemonImage(BattlePokemonModel pokemon)
        {
            PlayerPokemonImage = await _uiFacade.LoadPokemonBackSpritePathAsync(pokemon.PartyPokemon.Name);
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
        public BattleViewModel(UIFacade uIFacade, ListPokemonDisplayModelFactory displayModelFactory, IBattleService battleService, TypeDataService typeDataService, BattleFacade battlefacade)
        {
           _typeDataService = typeDataService;
            _uiFacade = uIFacade;
            _battleFacade = battlefacade;
            _battleService = battleService;
            _displayModelFactory = displayModelFactory;
          
            _ = LoadGraphics();
            ClickMoveCommand = new Command<string>(async (moveName) => await OnClickMoveCommand(moveName));

            StartMatch();
            //DisplayTeamPokemon = new ObservableCollection<ListPokemonDisplayModel> _uiFacade.GetPokemonTeamAsync();
        }

        private async Task StartMatch()
        {
            var turnResult = await _battleFacade.StartMatch();

            if (turnResult == null)
            {
                throw new Exception("Something fucked up");
            }
            CurrentPlayerPartyPokemon = turnResult.PlayerCurrentPokemon;
            CurrentAiPokemon = turnResult.AiCurrentPokemon;
            await RebuildTeamDisplay();
        }

        //Härifrån, battleservice/Facade för att hantera skada fiende
        //Sen AI
        //Sen mer grafik
        //Färdig?
        //Crazy
        private async Task OnClickMoveCommand(string moveName)
        {
            Console.WriteLine($"Clicked move {moveName}");
            var turnResult = await _battleFacade.NewTurn(moveName);

            CurrentPlayerPartyPokemon = turnResult.PlayerCurrentPokemon;
            CurrentAiPokemon = turnResult.AiCurrentPokemon;
        }

        public async Task RebuildTeamDisplay()
        {
            DisplayPlayerParty.Clear();
            var battlemon = _battleFacade.CurrentPlayerPokemon;
            var curMoves = CurrentPlayerPartyPokemon.PartyPokemon.Moves.Select(e => e.Name);

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
            foreach (var pokemin in pokemon)
            {
                var display = await _displayModelFactory.CreateFrontFacingSprite(pokemin);
                opponent.Add(display.SpritePath);
            }
            Console.WriteLine(PlayerPokemonImage);
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
