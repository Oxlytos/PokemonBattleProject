using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Domain.Models.RequestModels;
using Pokemon.Infrastructure.Interfaces;

namespace Pokemon.Infrastructure.Services
{
    public class TeamPokemonService : ITeamPokemonService
    {
        public ObservableCollection<PartyPokemonModel> TeamPokemon { get; set; } = new();
        public async Task AddToTeam(PartyPokemonModel pokemon)
        {
            if (TeamPokemon.Count < 6)
            {
                pokemon.Id = TeamPokemon.Count+1;
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

        public async Task RemoveFromTeam(PartyPokemonModel pokemon)
        {
            if(TeamPokemon.Count > 0)
            {
                TeamPokemon.Remove(pokemon);
                await Task.CompletedTask;

            }
        }

        public async void UpdateTeamMember(PartyPokemonModel pokemon)
        {
            var thisPokemon = TeamPokemon.FirstOrDefault(p=>p.Id==pokemon.Id);

            if (thisPokemon != null)
            {
                thisPokemon.Moves = pokemon.Moves;
                Console.WriteLine(thisPokemon.Moves);
            }
        }
    }
}
