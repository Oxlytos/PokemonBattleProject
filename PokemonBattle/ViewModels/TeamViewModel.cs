using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Domain.Models;
using Domain.Models.Models;
using Pokemon.Infrastructure.Interfaces;
using Pokemon.Infrastructure.Services;
using Pokemon.Services.Interfaces;
using Pokemon.Shared.Extensions;
using PokemonBattle.Interfaces;
using PokemonBattle.ListModel;

namespace PokemonBattle.ViewModels
{
    public class TeamViewModel : ITeamViewModel, INotifyPropertyChanged
    {
        private IPokemonFetchService _fetchService;
        private IImageService _imageService;
        private ITeamPokemonService _teamPokemonService;
        private ITypeService _typeService;
        private IMauiStorageDirectoryHelper _mauiStorageDirectoryHelper;
        public ObservableCollection<PokemonModel> AllPokemon { get; }
        public ObservableCollection<PokemonModel> TeamPokemon => _teamPokemonService.TeamPokemon;
        public ObservableCollection<ListPokemonDisplayModel> DisplayTeamPokemon { get;  } = new();
        private ObservableCollection<MoveRequestModel> _moves;
        public ObservableCollection<MoveRequestModel>? BasicMovesModels
        {
            get
            {
                return _moves;
            }
            set
            {
                _moves = value;
                OnPropertyChanged(nameof(BasicMovesModels));

            }
        }
        public ICommand GetPokemonCommand { get; }
        public ICommand AddToTeamCommand { get; }
        public ICommand RemoveFromTeamCommand { get; }
        public ICommand GoToHomeMenuCommand { get; }
        public ICommand GoToMoveAssignerPageCommand { get; }
        public ICommand GetMovesCommand { get; }

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

        public event PropertyChangedEventHandler? PropertyChanged;
        private string _pokemonName { get; set; }
        public string PokemonName
        {
            get { return _pokemonName; }
        }

        private PokemonModel _selectedPokemonModel;
        public PokemonModel SelectedPokemonModel
        {
            get { return _selectedPokemonModel; }
            set
            {
                _selectedPokemonModel = value;
                _pokemonName = value?.Name;
                OnPropertyChanged(nameof(SelectedPokemonModel));
                OnPropertyChanged(nameof(PokemonName));

                ((Command)AddToTeamCommand).ChangeCanExecute();
                //Command load image?
                _ = LoadPokemonSpriteAsync();
            }
        }
        public TeamViewModel(
            IPokemonFetchService pokemonFetchService,
            IImageService imageService,
            ITeamPokemonService teamPokemonService,
            ITypeService typeService,
            IMauiStorageDirectoryHelper mauiStorageDirectoryHelper

            )
        {
            _mauiStorageDirectoryHelper = mauiStorageDirectoryHelper;
            _fetchService = pokemonFetchService;
            _imageService = imageService;
            _teamPokemonService = teamPokemonService;
            _typeService = typeService;


            AllPokemon = new ObservableCollection<PokemonModel>();
            GetPokemonCommand = new Command(async () => await LoadPokemonAsync());
            AddToTeamCommand = new Command(async () => await AddToTeam(), () => SelectedPokemonModel != null);
            RemoveFromTeamCommand = new Command<ListPokemonDisplayModel>(async (poke) => await RemoveFromTeam(poke));
            GoToMoveAssignerPageCommand = new Command(async () => await GoToMoveAssignerPage());
            GetMovesCommand = new Command(async() => await GetPokemonRequestModelMoves());
        }
        public async Task GetPokemonRequestModelMoves()
        {
            if(SelectedPokemonModel != null)
            {
                var movesList = SelectedPokemonModel.Moves?.Moves ?? new MoveRequestModel[1];
                BasicMovesModels = new ObservableCollection<MoveRequestModel>(movesList);

            }
        }
        public async Task GoToMoveAssignerPage()
        {
            var page = new MoveAssignerPage(SelectedPokemonModel, this);
            await Shell.Current.Navigation.PushAsync(page);
        }
        public async Task GoToBattlePage()
        {
            await Shell.Current.Navigation.PopToRootAsync();
        }
        public async Task RemoveFromTeam(ListPokemonDisplayModel listpokmeon)
        {
            if (listpokmeon == null)
            {
                return;
            }

            if (DisplayTeamPokemon.Contains(listpokmeon))
            {
                var index = DisplayTeamPokemon.IndexOf(listpokmeon);
                DisplayTeamPokemon.RemoveAt(index);
                var thisPokemon = _teamPokemonService.TeamPokemon[index];
                await _teamPokemonService.RemoveFromTeam(thisPokemon);
            }
        }
        public async Task LoadPokemonSpriteAsync()
        {
            //har vi tryckt på en pokemon i listan, annars returnera
            if (SelectedPokemonModel == null) return;

            //vägen dit (om den finns)
            var path = await _imageService.GetSpritePath(SelectedPokemonModel.Name, "front_default.png");
            var fullPokemonInfo = await _fetchService.GetPokemonSingularAsync(_pokemonName);
            SelectedPokemonModel.Moves = fullPokemonInfo.Moves;
            if (fullPokemonInfo?.Sprites != null && !_imageService.AreAllSpritesStored(_pokemonName))
            {
                await _imageService.SaveImage(_pokemonName, fullPokemonInfo.Sprites.SpriteModel);
            }

            if (File.Exists(path))
            {
                PokemonImage = ImageSource.FromFile(path);
            }

        }
        public async Task LoadSpriteForPokemonListItemAsync(ListPokemonDisplayModel listItem)
        {
            if (listItem == null) return;

            var pokemonName = listItem.Name;
            var fullPokemonInfo = await _fetchService.GetPokemonSingularAsync(pokemonName);

            SelectedPokemonModel.Moves = fullPokemonInfo.Moves;
            var spritePath = await _imageService.GetSpritePath(pokemonName, "front_default.png");


            var typeList = fullPokemonInfo.Types.Select(t => t.Types.Name).ToList();
            var typeSpriteList = new List<string>();

            foreach (var type in typeList)
            {
                var fullTypeInfo = await _typeService.GetTypeData(type);

                await _imageService.SaveTypeSprite(type, fullTypeInfo.Sprites);

                var path = await _imageService.GetTypeSpriteFolder(type);
                typeSpriteList.Add(path);
            }

            listItem.SpriteTypePaths = typeSpriteList.ToArray();
            if (fullPokemonInfo?.Sprites != null && !_imageService.AreAllSpritesStored(pokemonName))
            {
                await _imageService.SaveImage(pokemonName, fullPokemonInfo.Sprites.SpriteModel);
            }

            if (File.Exists(spritePath))
            {
                //Actual sprite som vin bindar till
                listItem.SpritePath = spritePath;

                PokemonImage = ImageSource.FromFile(spritePath);
            }


        }
        public async Task AddToTeam()
        {
            if (_selectedPokemonModel == null)
            {
                return;
            }

            await _teamPokemonService.AddToTeam(_selectedPokemonModel);

            var displayTeamMember = new ListPokemonDisplayModel(_selectedPokemonModel);
            if (DisplayTeamPokemon.Count >= TeamPokemon.Count)
            {

            }
            else
            {
                DisplayTeamPokemon.Add(displayTeamMember);
                _ = LoadSpriteForPokemonListItemAsync(displayTeamMember);
            }

            //SelectedPokemonModel = null;
        }
        public async Task LoadPokemonAsync()
        {
            var pokemon = await _fetchService.GetPokemonAsync();
            AllPokemon.Clear();
            foreach (var item in pokemon)
            {
                AllPokemon.Add(item);
            }
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public Task LoadTeamAsync()
        {
            throw new NotImplementedException();
        }
    }
}
