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
using PokemonBattle.Interfaces;
using PokemonBattle.ListModel;

namespace PokemonBattle.ViewModels
{
    public class MoveViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

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
                _actualPokemon = value;
                SelectedPokemonModel = new ListPokemonDisplayModel()
                OnPropertyChanged(nameof(ActualPokemon));
            }
        }
        private ListPokemonDisplayModel _selectedPokemonModel;
        public ListPokemonDisplayModel SelectedPokemonModel
        {
            get { return _selectedPokemonModel; }
            set
            {
                _selectedPokemonModel = value;
                _pokemonName = value?.Name;
                OnPropertyChanged(nameof(SelectedPokemonModel));
                OnPropertyChanged(nameof(PokemonName));

                //Command load image?
                //_ = LoadPokemonSpriteAsync();
            }
        }

        private ObservableCollection<RequestMoveModel>? _currentMoves;
        public ObservableCollection<RequestMoveModel>? CurrentMoves
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
            IMoveService moveService
            )
        {
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

        private void GetCorrectImage()
        {
            if(SelectedPokemonModel != null)
            {
                var path = _imageService.GetSpritePath(SelectedPokemonModel.Name, "front_default.png");

            }

        }

        private async Task AddMoveToPokemon()
        {
            var moves = await _moveService.AddMove(SelectedPokemonModel.Pokemon, CurrentMove);
            
            //räkna från högsta tal, neråt, -1 för det är array logik
            for(int i = CurrentMoves.Count-1; i >= 0; i--)
            {
                //Är inte de synkade med index och move
                if (!moves.Contains(CurrentMoves[i]))
                {
                    //ta bort
                    CurrentMoves.RemoveAt(i);
                }

            }
            //Kolla alla nuvarande lerarned moves
            foreach(var move in moves)
            {
                //Om inte UI moves har object moves
                if (!CurrentMoves.Contains(move))
                {
                    //Spara bara move till senare
                    var moveData = await _fetchService.GetMoveModelAsync(move.Move.Name);
                    Console.WriteLine(moveData);
                    //Lägg till
                    move.Power = moveData.Power != null ? (int?)moveData.Power : null;
                    move.TypeName =  "To be addressed";
                    CurrentMoves.Add(move);
                }
            }
            //Fungerar som länk/pointer till riktiga moves?
            SelectedPokemonModel.Pokemon.LearnedMoves = CurrentMoves.ToArray();
            _teamPokemonService.UpdateTeamMember(SelectedPokemonModel.Pokemon);
            //await RefreshCurrentMoves();
        }
        private async Task RefreshCurrentMoves()
        {
            var currentMoves = SelectedPokemonModel.Pokemon.LearnedMoves ?? new RequestMoveModel[4];
            CurrentMoves = new ObservableCollection<RequestMoveModel>(currentMoves);
        }

        public async Task GetPokemonRequestModelMoves()
        {
            if (SelectedPokemonModel != null)
            {
                var pokemonData = await _fetchService.GetPokemonSingularAsync(SelectedPokemonModel.Name);

                var currentMoves = pokemonData.LearnedMoves ?? new RequestMoveModel[4];
                foreach(var move in currentMoves)
                {
                    move.Move.Name = move.Move.Name.Capitalize();
                }
                CurrentMoves = new ObservableCollection<RequestMoveModel>(currentMoves);
                

                var movesList = pokemonData.Moves?.Moves ?? new RequestMoveModel[1];
                foreach(var move in movesList)
                {
                    move.Move.Name = move.Move.Name.Capitalize();
                }
                
                AvailableMoves = new ObservableCollection<RequestMoveModel>(movesList);
            }
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
