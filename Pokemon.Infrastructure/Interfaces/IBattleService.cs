using Domain.Models.Game;

namespace Pokemon.Infrastructure.Interfaces
{
    public interface IBattleService
    {
        Task PlayerMove();
        Task AIMove();
        Task<PartyPokemonModel> GetFirstPartyPokemon();
        Task LoadPartyPokemon();
    }
}