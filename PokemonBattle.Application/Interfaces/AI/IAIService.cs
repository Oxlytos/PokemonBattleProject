using Domain.Models.Game;
using Pokemon.Infrastructure.Models;

namespace Pokemon.Infrastructure.Interfaces.AI
{
    public interface IAIService
    {
        Task<MoveModel> AIChoosesMove(BattlePokemonModel player, BattlePokemonModel aiPokemon);

    }
}