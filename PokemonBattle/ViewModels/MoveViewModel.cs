using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Domain.Models.Game;
using Domain.Models.Models;
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
        private readonly UIFacade _uiFacade;

        private IPokemonFetchService _fetchService;
        private IImageService _imageService;
        private ITeamPokemonService _teamPokemonService;
        private ITypeService _typeService;
        private IMauiStorageDirectoryHelper _mauiStorageDirectoryHelper;
        private IMoveService _moveService;
        public ICommand GetMovesCommand { get; }
        public ICommand AddMoveCommand { get; }

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
            IPokemonFetchService pokemonFetchService,
            IImageService imageService,
            ITeamPokemonService teamPokemonService,
            ITypeService typeService,
            IMauiStorageDirectoryHelper mauiStorageDirectoryHelper,
            IMoveService moveService,
            UIFacade uIFacade
            )
        {
            _uiFacade = uIFacade;
            _mauiStorageDirectoryHelper = mauiStorageDirectoryHelper;
            _fetchService = pokemonFetchService;
            _imageService = imageService;
            _teamPokemonService = teamPokemonService;
            _typeService = typeService;
            _moveService = moveService;
            
            GetMovesCommand = new Command(async () => await GetPokemonRequestModelMoves());
            AddMoveCommand = new Command(async () => await AddMoveToPokemon(), () => CurrentMove != null);
            GetCorrectImage();
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
            List<ListMoveDisplayModel> listMoves = new List<ListMoveDisplayModel>();
            foreach (var move in moves)
            {
                ListMoveDisplayModel newMove = new ListMoveDisplayModel(move);
                if (!string.IsNullOrEmpty(newMove.Name))
                {
                    var typeInfo = await _fetchService.GetMoveModelAsync(newMove.Name);
                    newMove.TypeName = typeInfo.MoveTypeInfo.Name.Capitalize();
                    newMove.Name=newMove.Name.Capitalize();
                }
                listMoves.Add(newMove);
            }

            CurrentMoves = new ObservableCollection<ListMoveDisplayModel>(listMoves);
            foreach (var move in CurrentMoves)
            {
                move.Name = move.Name.Capitalize();
                Console.WriteLine(move.Name);
            }
            foreach (var move in CurrentMoves)
            {
                Console.WriteLine(move.Name);
            }
            Console.WriteLine(CurrentMoves);
            OnPropertyChanged(nameof(CurrentMoves));
        }

        private void GetCorrectImage()
        {
            if (SelectedPokemonModel != null)
            {
                var path = _imageService.GetPokemonSpriteAsyncPNG(SelectedPokemonModel.Name);

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
            var moveToAdd = await _uiFacade.AddMoveAsync(CurrentMove, _actualPokemon);
            if (moveToAdd == null)
            {
                return ;
            }
            CurrentMoves.Add(moveToAdd);
            SelectedPokemonModel.DisplayMoves = CurrentMoves.ToArray();
            var moves = SelectedPokemonModel.DisplayMoves;
            Console.WriteLine(moves);

            //Se till att moves stämmar överenns
            for (int i = CurrentMoves.Count - 1; i >= 0; i--)
            {
                //Är inte de synkade med index och move
                if (!moves.Contains(CurrentMoves[i]))
                {
                    //ta bort
                    CurrentMoves.RemoveAt(i);
                }

            }
            //Kolla alla nuvarande lerarned moves
            foreach (var move in moves)
            {
                //Om inte UI moves har object moves
                if (!CurrentMoves.Contains(move))
                {
                    CurrentMoves.Add(move);
                }
            }
            //Fungerar som länk/pointer till riktiga moves?
            SelectedPokemonModel.DisplayMoves = CurrentMoves.ToArray();
            _teamPokemonService.UpdateTeamMember(ActualPokemon);
            RenderCurrentMoves();
        }
      
        public async Task GetPokemonRequestModelMoves()
        {
            if (SelectedPokemonModel == null)
            {
                return;
            }
            //request modellen
            var pokemonData = await _fetchService.GetPokemonSingularAsync(SelectedPokemonModel.Name);

            //ritkiga moves som blir display sen
            List<MoveModel> moves = new List<MoveModel>();

            //request
            var currentMoves = pokemonData.LearnedMoves ?? new RequestMoveModel[4];
            foreach (var move in currentMoves)
            {
                move.Move.Name = move.Move.Name.Capitalize();
                var actualMove = await _fetchService.GetSerialisedMoveModelAsync(move.Move.Name);
                moves.Add(actualMove);
            }


            var movesList = pokemonData.Moves?.Moves ?? new RequestMoveModel[1];
            foreach (var move in movesList)
            {
                move.Move.Name = move.Move.Name.Capitalize();
            }
          
            AvailableMoves = new ObservableCollection<RequestMoveModel>(movesList);
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
