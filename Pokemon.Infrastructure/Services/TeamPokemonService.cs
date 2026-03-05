using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Models;
using Pokemon.Infrastructure.Interfaces;

namespace Pokemon.Infrastructure.Services
{
    public class TeamPokemonService : ITeamPokemonService
    {
        public ObservableCollection<PokemonModel> TeamPokemon { get; } = new();
        public async Task AddToTeam(PokemonModel pokemon)
        {
            if (TeamPokemon.Count < 6)
            {
                TeamPokemon.Add(pokemon);
                 await Task.CompletedTask;
            }

        }
        public async Task RemoveFromTeam(PokemonModel pokemon)
        {
            if(TeamPokemon.Count > 0)
            {
                TeamPokemon.Remove(pokemon);
                await Task.CompletedTask;

            }
        }
    }
}
