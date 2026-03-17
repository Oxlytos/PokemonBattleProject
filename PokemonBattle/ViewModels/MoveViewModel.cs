using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Domain.Models.Game;
using Domain.Models.RequestModels;
using Pokemon.Infrastructure.Interfaces;
using Pokemon.Services.Interfaces;
using Pokemon.Shared.Extensions;
using PokemonBattle.Facades;
using PokemonBattle.Interfaces;
using PokemonBattle.ListModel;

namespace PokemonBattle.ViewModels
{
    public class MoveViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly MoveFacade _moveFacade;

        public ICommand GetMovesCommand { get; }
        public ICommand AddMoveCommand { get; }
        public ICommand RemoveMoveCommand { get; }

        private ImageSource _pokemonImage;
        public ImageSource PokemonImage
        {
            get
            {
                return _pokemonImage;
            }
            set
            {
                if (_pokemonImage != value)
                {
                    _pokemonImage = value;
                    OnPropertyChanged(nameof(PokemonImage));
                }
            }
        }
        private string _pokemonName { get; set; }
        public string PokemonName
        {
            get { return _pokemonName; }
        }
        private PartyPokemonModel _actualPokemon;
        public PartyPokemonModel ActualPokemon
        {
            get
            {
                return _actualPokemon;
            }
            set
            {
                if(_actualPokemon == value)
                {
                    return;
                }
                _actualPokemon = value;
                SelectedPokemonModel = _actualPokemon != null ? new ListPokemonDisplayModel(_actualPokemon) : null;
                OnPropertyChanged(nameof(ActualPokemon));
                RenderCurrentMoves();
            }
        }
        private ListPokemonDisplayModel _selectedPokemonModel;
        public ListPokemonDisplayModel SelectedPokemonModel
        {
            get => _selectedPokemonModel;
            private set
            {
                _selectedPokemonModel = value;
                _pokemonName = value?.Name;
                OnPropertyChanged(nameof(SelectedPokemonModel));
            }
        }

        //Enbart listor för display
        private ObservableCollection<ListMoveDisplayModel>? _currentMoves;
        public ObservableCollection<ListMoveDisplayModel>? CurrentMoves
        {
            get
            {
                return _currentMoves;
            }
            set
            {
                _currentMoves = value;
                OnPropertyChanged(nameof(CurrentMoves));

            }
        }

        private ObservableCollection<RequestMoveModel> _aMoves;
        public ObservableCollection<RequestMoveModel>? AvailableMoves
        {
            get
            {
                return _aMoves;
            }
            set
            {
                _aMoves = value;
                OnPropertyChanged(nameof(AvailableMoves));

            }
        }
        private RequestMoveModel _currentMove;
        public RequestMoveModel CurrentMove
        {
            get
            {
                return _currentMove;
            }
            set
            {
                _currentMove = value;
                OnPropertyChanged(nameof(CurrentMove));
                ((Command)AddMoveCommand).ChangeCanExecute();
            }
        }
        public MoveViewModel(
            MoveFacade moveFacade,
            UIFacade uIFacade
            )
        {
            _moveFacade = moveFacade;
            
            GetMovesCommand = new Command(async () => await GetPokemonRequestModelMoves());
            AddMoveCommand = new Command(async () => await AddMoveToPokemon(), () => CurrentMove != null);
            RemoveMoveCommand = new Command<ListMoveDisplayModel>(async(move) => await RemoveMoveFromPokemon(move));
            GetCorrectImage();
        }

        private async Task RemoveMoveFromPokemon(ListMoveDisplayModel move)
        {
            if (move == null)
            {
                return;
            }
           
            if (ActualPokemon == null)
            {
                return;
            }
           
            ActualPokemon = await _moveFacade.RemoveMoveFromPokemon(ActualPokemon, move);
            await RenderCurrentMoves();

        }

        private async Task RenderCurrentMoves()
        {
            if (ActualPokemon == null)
            {
                return;
            }
            var moves = ActualPokemon.Moves;
            if (moves == null)
            {
                return;
            }
            CurrentMoves = await  _moveFacade.UpdateCurrentMovesDisplay(moves);
            OnPropertyChanged(nameof(CurrentMoves));
        }

        private void GetCorrectImage()
        {
            if (SelectedPokemonModel != null)
            {
                var path = _moveFacade.GetPokemonSpriteAsyncPNG(SelectedPokemonModel.Name);
            }
        }

        private async Task AddMoveToPokemon()
        {
            if (_actualPokemon == null)
            {
                return;
            }
            if(CurrentMove == null)
            {
                return;
            }
            //Lägga till move till display list item
            //Och även den riktiga datamodellen
            //Eller rättare sagt, först party, sen den andra
            var moveToAdd = await _moveFacade.AddMoveAsync(CurrentMove, _actualPokemon);
            if (moveToAdd == null)
            {
                return ;
            }
            CurrentMoves.Add(moveToAdd);
            SelectedPokemonModel.DisplayMoves = CurrentMoves.ToArray();
            await RenderCurrentMoves();

        }
      
        public async Task GetPokemonRequestModelMoves()
        {
            if (SelectedPokemonModel == null && SelectedPokemonModel.Name ==null)
            {
                return;
            }
            AvailableMoves = await _moveFacade.GetAvailableMoves(SelectedPokemonModel.Name);
           
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
