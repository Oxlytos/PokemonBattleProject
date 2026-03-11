using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Domain.Models.Base;
using Domain.Models.Game;
using Domain.Models.RequestModels;
using Microsoft.Maui.Controls;
using Pokemon.Infrastructure.Interfaces;
using Pokemon.Infrastructure.Services;
using Pokemon.Repository.Interfaces;
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
        private IJsonStorage _jsonStorage;
        public ObservableCollection<RequestPokeonModel> AllPokemon { get; }
        public ObservableCollection<RequestPokeonModel> TeamPokemon => _teamPokemonService.TeamPokemon;
        public ObservableCollection<ListPokemonDisplayModel> DisplayTeamPokemon { get;  } = new();
        private ObservableCollection<RequestMoveModel> _moves;
        public ObservableCollection<RequestMoveModel>? BasicMovesModels
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

        public ICommand SaveTeamCommand { get; }
        public ICommand LoadTeamCommand { get; }

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
        private RequestPokeonModel _teamPokemonModelSelected;
        public RequestPokeonModel TeamPokemonModelSelected
        {
            get { return _teamPokemonModelSelected; }
            set
            {
                _teamPokemonModelSelected = value;
                OnPropertyChanged(nameof(TeamPokemonModelSelected));
            }
        }
        private RequestPokeonModel _selectedPokemonModel;
        public RequestPokeonModel SelectedPokemonModel
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
            IMauiStorageDirectoryHelper mauiStorageDirectoryHelper,
            IJsonStorage jsonStorage
            )
        {
            //SelectedPokemonModel = null;
            _mauiStorageDirectoryHelper = mauiStorageDirectoryHelper;
            _fetchService = pokemonFetchService;
            _imageService = imageService;
            _teamPokemonService = teamPokemonService;
            _typeService = typeService;
            _jsonStorage = jsonStorage;

            AllPokemon = new ObservableCollection<RequestPokeonModel>();
            GetPokemonCommand = new Command(async () => await LoadPokemonAsync());
            AddToTeamCommand = new Command(async () => await AddToTeam(), () => SelectedPokemonModel != null);
            RemoveFromTeamCommand = new Command<ListPokemonDisplayModel>(async (poke) => await RemoveFromTeam(poke));
            GoToMoveAssignerPageCommand = new Command<ListPokemonDisplayModel>(async (poke) => await GoToMoveAssignerPage(poke));
            SaveTeamCommand = new Command(async() => await SaveTeam(_teamPokemonService.TeamPokemon.ToList()));
            LoadTeamCommand = new Command(async () => await LoadTeamAsync());
            GetMovesCommand = new Command(async () => await GetPokemonRequestModelMoves());
        }
        public async Task RebuildTeamDisplay()
        {
            DisplayTeamPokemon.Clear();
            foreach (var pokemin in TeamPokemon)
            {
                var datapoke = await _fetchService.GetPokemonSingularAsync(pokemin.Name);
                var sprit = await _imageService.GetPokemonSpriteAsyncPNG(pokemin.Name);
                var typeNames = datapoke.Types.Select(t => t.Types.Name).ToArray();

                var display = new ListPokemonDisplayModel(datapoke);
                display.SpritePath = sprit;
                display.SpriteTypePaths = await _imageService.GetTypeSprite(typeNames);
                display.Nickname = pokemin.Nickname;
                OnPropertyChanged(nameof(Pokemon));
                DisplayTeamPokemon.Add(display);
            }
        }

        private async Task SaveTeam(List<PartyPokemonModel> pokemonTeam)
        {
           
            await _jsonStorage.SaveTeamAsync(pokemonTeam);
        }
        public async Task LoadTeamAsync()
        {
            var team = await _jsonStorage.LoadTeamAsync();
            if(team!= null)
            {
                _teamPokemonService.TeamPokemon.Clear();
                foreach (var pokemon in team)
                {
                    _teamPokemonService.TeamPokemon.Add(pokemon);
                }
            }
            await RebuildTeamDisplay();

        }

        public async Task GetPokemonRequestModelMoves()
        {
            if(SelectedPokemonModel != null)
            {
                var movesList = SelectedPokemonModel.Moves?.Moves ?? new RequestMoveModel[1];
                BasicMovesModels = new ObservableCollection<RequestMoveModel>(movesList);

            }
        }
        public async Task GoToMoveAssignerPage(ListPokemonDisplayModel listmodel)
        {
            if (listmodel != null)
            {
                var moveView = App.Current.Handler.MauiContext.Services.GetService<MoveViewModel>();

                moveView.SelectedPokemonModel = listmodel;
                PokemonImage = await _imageService.GetPokemonSpriteAsyncPNG(listmodel.Name);
                moveView.PokemonImage = PokemonImage;
                var page = new MoveAssignerPage(moveView);
                await Shell.Current.Navigation.PushAsync(page);
            }
           
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

            //finns inte
            if (!_imageService.AreAllSpritesStored(_pokemonName))
            {

                var fullPokemonInfo = await _fetchService.GetPokemonSingularAsync(_pokemonName);
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

            //data
            var pokemon = await _fetchService.GetPokemonSingularAsync(listItem.Name);
            Console.WriteLine(pokemon);
            //sprite png
            listItem.SpritePath = await _imageService.GetPokemonSpriteAsyncPNG(pokemon.Name);
            Console.WriteLine(listItem.SpritePath);
            //namn
            var typeNames = pokemon.Types.Select(t=>t.Types.Name).ToArray();

            listItem.SpriteTypePaths = (await _imageService.GetTypeSprite(typeNames));

        }
        public async Task AddToTeam()
        {
            if (_selectedPokemonModel == null)
            {
                return;
            }
            if (!await _teamPokemonService.CanWeAddToTeam())
            {
                return;
            }

            //konstructor eller metod för konverterng, som bara borde vara en deseralise av request
            var partyPokemon = new PartyPokemonModel();
            await _teamPokemonService.AddToTeam(partyPokemon);

            var displayTeamMember = new ListPokemonDisplayModel(_selectedPokemonModel);
            if (DisplayTeamPokemon.Count >= TeamPokemon.Count)
            {

            }
            else
            {
                DisplayTeamPokemon.Add(displayTeamMember);
                await LoadSpriteForPokemonListItemAsync(displayTeamMember);
            }

            SelectedPokemonModel = null;
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
      
    }
}
