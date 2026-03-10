using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Models;

namespace Pokemon.Infrastructure.Interfaces
{
    public interface ITeamPokemonService
    {
        public ObservableCollection<PokemonModel> TeamPokemon { get; }
        public Task AddToTeam(PokemonModel pokemon);
        public Task RemoveFromTeam(PokemonModel pokemon);

        public void UpdateTeamMember(PokemonModel pokemon);

        public Task<bool> CanWeAddToTeam();
    }
}
