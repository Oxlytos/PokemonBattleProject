using Domain.Models.Game;

namespace Pokemon.Infrastructure.Interfaces
{
    public interface IBattleService
    {
        Task<PartyPokemonModel> GetFirstPartyPokemon();
        Task LoadPartyPokemon();
    }
}