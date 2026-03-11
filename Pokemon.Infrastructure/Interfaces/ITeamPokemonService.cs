using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.RequestModels;

namespace Pokemon.Infrastructure.Interfaces
{
    public interface ITeamPokemonService
    {
        public ObservableCollection<RequestPokeonModel> TeamPokemon { get; }
        public Task AddToTeam(RequestPokeonModel pokemon);
        public Task RemoveFromTeam(RequestPokeonModel pokemon);

        public void UpdateTeamMember(RequestPokeonModel pokemon);

        public Task<bool> CanWeAddToTeam();
    }
}
