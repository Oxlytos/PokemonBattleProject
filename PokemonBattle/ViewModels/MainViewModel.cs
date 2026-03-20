using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Pokemon.Infrastructure.Interfaces;

namespace PokemonBattle.ViewModels
{
    public class MainViewModel:INotifyPropertyChanged
    {
        //Commands
        public ICommand GoToTeamBuilderPageCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;
        public MainViewModel(IFetchService pokemonFetchService, IImageService imageService)
        {
            GoToTeamBuilderPageCommand = new Command(async () => await GoToTeamBuilder());
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
