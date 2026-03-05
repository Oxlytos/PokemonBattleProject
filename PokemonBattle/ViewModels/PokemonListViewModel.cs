using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Pokemon.Services.Interfaces;
using Pokemon.Services.Services;
using PokemonBattle.Interfaces;

namespace PokemonBattle.ViewModels
{
    public class PokemonListViewModel:IPokemonListViewModel
    {
        private readonly IPokemonFetchService _fetchService;
        public ICommand GetPokemonAsyncCommand { get; }

        public PokemonListViewModel(IPokemonFetchService pokemonFetchService)
        {
            _fetchService = pokemonFetchService;

            GetPokemonAsyncCommand = new Command(async () => await GetPokemonAsync());
        }
        public async Task GetPokemonAsync()
        {
            var pokemon = await _fetchService.GetPokemonAsync();

        }
    }
}
