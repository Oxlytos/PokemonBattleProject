using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Domain.Models.Models;
using Pokemon.Services.Interfaces;

namespace PokemonBattle.ViewModels
{
    public class MainViewModel:INotifyPropertyChanged
    {
        public ICommand GoToTeamBuilderPageCommand { get; }




        private readonly IPokemonFetchService _fetchService;
        private readonly IImageService _imageService;
        public ObservableCollection<PokemonModel> Pokemon { get; set; } = new ObservableCollection<PokemonModel>();
        public ObservableCollection<PokemonModel> TeamPokemon { get; set; } = new ObservableCollection<PokemonModel>();

        //Commands
        public ICommand AddToTeamCommand { get; }
        public ICommand RemoveFromTeamCommand { get; }

        private string _pokemonName;

        public event PropertyChangedEventHandler? PropertyChanged;
        public MainViewModel(IPokemonFetchService pokemonFetchService, IImageService imageService)
        {
            _fetchService = pokemonFetchService;
            _imageService = imageService;

            GoToTeamBuilderPageCommand = new Command(async () => await GoToTeamBuilder());
        }

        public string PokemonName
        {
            get 
            { 
                return _pokemonName; 
            }
            set 
            { 
                _pokemonName = value;
                OnPropertyChanged(nameof(PokemonName));
            }
        }

        private PokemonModel _selectedPokemon;

        public PokemonModel SelectedPokemon
        {
            get
            {
                return _selectedPokemon;
            }
            set
            {
                _selectedPokemon = value;
                OnPropertyChanged(nameof(SelectedPokemon));
                ((Command)AddToTeamCommand).ChangeCanExecute(); //vi kan möjligen exekvera kommandot add to team nu
                _= LoadSprite(value); //_ discard
            }
        }

        public string _pokemonImage;
        public string PokemonImage
        {
            get
            {
                return _pokemonImage;
            }
            set
            {
                _pokemonImage = value;
                OnPropertyChanged(nameof(PokemonImage));
            }
        }

        public async Task LoadSprite(PokemonModel pokemonModel)
        {
            if(Pokemon == null)
            {
                return ;
            }
            PokemonName = pokemonModel.Name;


        }

        public async Task GoToTeamBuilder()
        {
            await Shell.Current.GoToAsync(nameof(TeamBuilderPage));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
