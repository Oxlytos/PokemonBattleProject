using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Domain.Models.Models;
using PokemonBattle.ListModel;

namespace PokemonBattle.Interfaces
{
    public interface ITeamViewModel
    {
        ICommand AddToTeamCommand { get; }
        ObservableCollection<PokemonModel> AllPokemon { get; }
        ObservableCollection<ListPokemonDisplayModel> DisplayTeamPokemon { get; }
        ICommand GetPokemonCommand { get; }
        ICommand GoToHomeMenuCommand { get; }
        ICommand GoToMoveAssignerPageCommand { get; }
        ImageSource PokemonImage { get; set; }
        string PokemonName { get; }
        ICommand RemoveFromTeamCommand { get; }
        PokemonModel SelectedPokemonModel { get; set; }
        ObservableCollection<PokemonModel> TeamPokemon { get; }

        event PropertyChangedEventHandler? PropertyChanged;

        Task AddToTeam();
        Task GoToBattlePage();
        Task GoToMoveAssignerPage();
        Task LoadPokemonAsync();
        Task LoadPokemonSpriteAsync();
        Task LoadSpriteForPokemonListItemAsync(ListPokemonDisplayModel listItem);
        Task LoadTeamAsync();
        Task RemoveFromTeam(ListPokemonDisplayModel listpokmeon);
    }
}
