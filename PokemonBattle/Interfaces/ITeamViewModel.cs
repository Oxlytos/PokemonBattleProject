using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Domain.Models.Models;

namespace PokemonBattle.Interfaces
{
    public interface ITeamViewModel
    {
        public ObservableCollection<PokemonModel> TeamPokemon {  get; }

        public ICommand GoToHomeMenuCommand { get; }
        public ICommand GetPokemonCommand { get; }
        public ICommand AddToTeamCommand { get; }
        public ICommand RemoveFromTeamCommand { get; }
        Task LoadPokemonAsync();
        Task LoadTeamAsync();
    }
}
