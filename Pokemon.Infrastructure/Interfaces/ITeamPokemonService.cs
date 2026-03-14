using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Domain.Models.RequestModels;

namespace Pokemon.Infrastructure.Interfaces
{
    public interface ITeamPokemonService
    {
        public ObservableCollection<PartyPokemonModel> TeamPokemon { get; }
        public Task AddToTeam(PartyPokemonModel pokemon);
        public Task RemoveFromTeam(PartyPokemonModel pokemon);
        public void UpdateTeamMember(PartyPokemonModel pokemon);
        public Task<bool> CanWeAddToTeam();
        public Task<PartyPokemonModel> GetFirstPartyPokemon();
    }
}
