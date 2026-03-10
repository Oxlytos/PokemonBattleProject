using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Models;
using Pokemon.Infrastructure.Interfaces;

namespace Pokemon.Infrastructure.Services
{
    public class TeamPokemonService : ITeamPokemonService
    {

        public ObservableCollection<PokemonModel> TeamPokemon { get; set; } = new();
        public async Task AddToTeam(PokemonModel pokemon)
        {
            if (TeamPokemon.Count < 6)
            {
                TeamPokemon.Add(pokemon);
                 await Task.CompletedTask;
            }

        }

        public async Task<bool> CanWeAddToTeam()
        {
            if (!TeamPokemon.Any())
            {
                return true;
            }
            if (TeamPokemon.Count >= 6)
            {
                return false;
            }
            return true;
        }

        public async Task RemoveFromTeam(PokemonModel pokemon)
        {
            if(TeamPokemon.Count > 0)
            {
                TeamPokemon.Remove(pokemon);
                await Task.CompletedTask;

            }
        }

        public async void UpdateTeamMember(PokemonModel pokemon)
        {
            var thisPokemon = TeamPokemon.FirstOrDefault(p=>p.Name==pokemon.Name);

            if (thisPokemon != null)
            {
                thisPokemon.LearnedMoves = pokemon.LearnedMoves;
                Console.WriteLine(thisPokemon.LearnedMoves);
            }
        }
    }
}
