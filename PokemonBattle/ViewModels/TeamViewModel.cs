using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Domain.Models.Models;
using Pokemon.Infrastructure.Interfaces;
using Pokemon.Services.Interfaces;
using Pokemon.Shared.Extensions;
using PokemonBattle.Interfaces;
using PokemonBattle.ListModel;

namespace PokemonBattle.ViewModels
{
    public class TeamViewModel:ITeamViewModel, INotifyPropertyChanged
    {
        private IPokemonFetchService _fetchService;
        private IImageService _imageService;
        private ITeamPokemonService _teamPokemonService;
        public ObservableCollection<PokemonModel> AllPokemon { get; }
        public ObservableCollection<PokemonModel> TeamPokemon => _teamPokemonService.TeamPokemon;
        public ObservableCollection<ListPokemonDisplayModel> DisplayTeamPokemon { get; } = new();
        public ICommand GetPokemonCommand { get; }
        public ICommand AddToTeamCommand { get; }
        public ICommand RemoveFromTeamCommand { get; }

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

        //De services som behövs
        public TeamViewModel(IPokemonFetchService pokemonFetchService, IImageService imageService, ITeamPokemonService teamPokemonService)
        {
            _fetchService = pokemonFetchService;
            _imageService = imageService;
            _teamPokemonService = teamPokemonService;


            AllPokemon = new ObservableCollection<PokemonModel>();
            GetPokemonCommand = new Command(async () => await LoadPokemonAsync());
            AddToTeamCommand = new Command(async () => await AddToTeam(), () => SelectedPokemonModel!=null);

            
        }
        public async Task LoadPokemonSpriteAsync()
        {
            //har vi tryckt på en pokemon i listan, annars returnera
            if (SelectedPokemonModel==null) return;
            
            //vägen dit (om den finns)
            var path = _imageService.GetSpritePath(SelectedPokemonModel.Name, "front_default.png");
            var fullPokemonInfo = await _fetchService.GetPokemonSingularAsync(_pokemonName);
            if(fullPokemonInfo?.Sprites!=null && !_imageService.AreAllSpritesStored(_pokemonName))
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
            if(listItem == null) return;

            var pokemonName = listItem.Name;
            var parth = _imageService.GetSpritePath(pokemonName, "front_default.png");
            var fullPokemonInfo = await _fetchService.GetPokemonSingularAsync(pokemonName);

            string types ="";

            types = string.Join("/", fullPokemonInfo.Types.Select(t=>t.Types.Name.Capitalize()));
            listItem.Types = types;
            if (fullPokemonInfo?.Sprites != null && !_imageService.AreAllSpritesStored(pokemonName))
            {
                await _imageService.SaveImage(pokemonName, fullPokemonInfo.Sprites.SpriteModel);
            }

            if (File.Exists(parth))
            {
                //Actual sprite som vin bindar till
                listItem.SpritePath = parth;
                
                PokemonImage = ImageSource.FromFile(parth);
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
            DisplayTeamPokemon.Add(displayTeamMember);

            _=LoadSpriteForPokemonListItemAsync(displayTeamMember);


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
        public Task LoadTeamAsync()
        {
            throw new NotImplementedException();
        }
    }
}
