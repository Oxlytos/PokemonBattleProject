using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Domain.Models.Game;
using Domain.Models.RequestModels;
using Pokemon.AppServices.Factories;
using Pokemon.Infrastructure.Models;
using PokemonBattle.Facades;
using PokemonBattle.ListModel;

namespace PokemonBattle.ViewModels
{
    public class BattleViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private BattleFacade _battleFacade;
        private readonly ListPokemonDisplayModelFactory _displayModelFactory;
        public ObservableCollection<RequestPokeonModel> AllPokemon { get; }
        public ObservableCollection<ListPokemonDisplayModel> DisplayPlayerParty { get; } = new();

        private ObservableCollection<ListMoveDisplayModel> _currentPlayerPokemonMoves;
        public ObservableCollection<ListMoveDisplayModel> CurrentPlayerPokemonMoves
        {
            get => _currentPlayerPokemonMoves;
            set
            {
                var padded = new ObservableCollection<ListMoveDisplayModel>(value ?? new ObservableCollection<ListMoveDisplayModel>());
                while (padded.Count < 4) 
                {
                    MoveModel moveModel = new MoveModel();
                    moveModel.Name = "-";
                    var emptyListMove = new ListMoveDisplayModel(moveModel);
                    padded.Add(emptyListMove);
                }
                _currentPlayerPokemonMoves = padded;
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
        //Hämta relevant bild, shiny eller inte
        private async Task GetOpponentPokemonImage(BattlePokemonModel pokemon)
        {
            if (pokemon != null)
            {
                string version = pokemon.IsShiny ? "front_shiny" : "front_default";
                OpponentPokemon = await _battleFacade.LoadPokemonFrontSpritePathAsync(pokemon.PartyPokemon.Name, version);

            }
        }

        //Shiny eller inte
        private async Task GetPokemonImage(BattlePokemonModel pokemon)
        {
            string version = pokemon.IsShiny ? "front_default" : "front_shiny";

            PlayerPokemonImage = await _battleFacade.LoadPokemonBackSpritePathAsync(pokemon.PartyPokemon.Name, version);
        }

        public ICommand ClickMoveCommand { get; }

        public ICommand SwitchPokemonCommand { get; }

        public ICommand ForfeitCommand { get; }

        public bool IsSwitching { get; set; }

        public bool CanInput { get; set; } = false;

        private string _statusMessage;
        public string StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                if ( _statusMessage != value )
                {
                    _statusMessage = value;
                    OnPropertyChanged(nameof(StatusMessage));
                }
            }
        }
        public BattleViewModel(ListPokemonDisplayModelFactory displayModelFactory, BattleFacade battlefacade)
        {
            _battleFacade = battlefacade;
            _displayModelFactory = displayModelFactory;
          
            ClickMoveCommand = new Command<string>(async (moveName) => await OnClickMoveCommand(moveName));
            SwitchPokemonCommand = new Command<ListPokemonDisplayModel>(async (listpokemon) => await OnClickSwitchPokemon(listpokemon));
            ForfeitCommand = new Command(async () => await OnClickForfeit());
            //Boolen används för att kalla eventen/voiden att sepelare måste btta pokemon
            _battleFacade.OnPlayerMustSwitch += HandlePlayerMustSwitch;

            //När modellen (battleviewmodel) skapas, starta matchen, alternativt någon stor knapp?
            StartMatch();
        }

        private async Task OnClickForfeit()
        {
            await Shell.Current.Navigation.PopToRootAsync();
            return;
        }

        private void HandlePlayerMustSwitch()
        {
            StatusMessage = "Your pokemon fainted! Choose a another one to keep battling!";
            IsSwitching = true;
        }

        //battle facade härifrån, se till att switch händer före move
        private async Task OnClickSwitchPokemon(ListPokemonDisplayModel listpokemon)
        {
            if(!IsSwitching) return;
            if (listpokemon == null)
            {
                return;
            }

            int index = DisplayPlayerParty.IndexOf(listpokemon);
            var changedPokemon = await _battleFacade.ChangeCurrentPokemon(index);
            if (changedPokemon.IsFainted)
            {
                StatusMessage = "Can't change in to a fainted pokemon....";
                await Task.Delay(1000);
                return;
            }
            if (changedPokemon != CurrentPlayerPartyPokemon)
            {
                CurrentPlayerPartyPokemon = changedPokemon;
                IsSwitching = false;
                await RebuildTeamDisplay();
            }

            IsSwitching = false;
            StatusMessage = "";
            await RebuildTeamDisplay();
        }

        private async Task StartMatch()
        {
            CanInput = true;
            var turnResult = await _battleFacade.StartMatch();

           
            CurrentPlayerPartyPokemon = turnResult.PlayerCurrentPokemon;
            CurrentAiPokemon = turnResult.AiCurrentPokemon;
            await RebuildTeamDisplay();
        }

        //När man klickar en giltig knapp
        private async Task OnClickMoveCommand(string moveName)
        {
            if(moveName == "-")
            {
                return;
            }
            if (IsSwitching)
            {
                return;
            }
            if (!CanInput)
            {
                return;
            }
            IsSwitching = false;
            var turnResult = await _battleFacade.NewTurn(moveName);

            CurrentPlayerPartyPokemon = turnResult.PlayerCurrentPokemon;
            CurrentAiPokemon = turnResult.AiCurrentPokemon;

            await PrintBattleInfo(turnResult.BattleActionMessages);
            await HandleTurnResult(turnResult);
           

        }

        //Hantera när någon vinner, eller faintar
        private async Task HandleTurnResult(TurnResult turnResult)
        {
            if (turnResult.PlayerWin || turnResult.AiWin)
            {
                string whoWon = "";
                if (turnResult.PlayerWin)
                {
                    OpponentPokemon = null;
                    whoWon = "Player";
                }
                if (turnResult.AiWin)
                {
                    PlayerPokemonImage = null;
                    whoWon = "AI";
                }
                StatusMessage = $"Match over! {whoWon} wins!";
                await Task.Delay(6000);
                await Shell.Current.Navigation.PopToRootAsync();
                return;
            }

            if (turnResult.PlayerCurrentPokemon.IsFainted)
            {
                StatusMessage = "Your pokemon fainted! Choose a another one to keep battling!";
                PlayerPokemonImage = null;

                IsSwitching = true;

                await Task.Delay(2000);
                return;
            }
            if (turnResult.AiCurrentPokemon.IsFainted)
            {
                StatusMessage = "AI is changing pokemon....";
                var newPokemon = await _battleFacade.AIChoosesNewPokemon();
                CurrentAiPokemon = newPokemon;
            }
            StatusMessage = "";
        }

        private async Task PrintBattleInfo(List<string> battleActionMessages)
        {
            //Gå igenom listan av saker som hänt i striden
            //2 sek delay så hinner man läsa
            CanInput = false;
            foreach (var message in battleActionMessages)
            {
                StatusMessage = message;
                await Task.Delay(2000); 
            }
            StatusMessage = "";
            CanInput = true;
        }

        public async Task RebuildTeamDisplay()
        {
            StatusMessage = "";
            DisplayPlayerParty.Clear();
            //CurrentPlayerPokemonMoves = await _battleFacade.GetCurrentPlayerMoves();
            CurrentPlayerPokemonMoves = await _battleFacade.GetCurrentPlayerMoves();
            Console.WriteLine(CurrentPlayerPokemonMoves);
            var pokemon = _battleFacade.PlayerTeam.ToList();
            foreach (var pokemin in pokemon)
            {
                var display = await _displayModelFactory.CreateFrontFacingSprite(pokemin.PartyPokemon);
                OnPropertyChanged(nameof(Pokemon));
                DisplayPlayerParty.Add(display);
            }

            var currentPokemonBackFacing = await _displayModelFactory.CreateBackFacingSprite(CurrentPlayerPartyPokemon.PartyPokemon);

            PlayerPokemonImage = ImageSource.FromFile(currentPokemonBackFacing.SpritePath);
            
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
