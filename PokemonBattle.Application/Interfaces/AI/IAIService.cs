using Domain.Models.Game;
using Pokemon.AppServices.Models;

namespace Pokemon.AppServices.Interfaces.AI
{
    public interface IAIService
    {
        Task<MoveModel> AIChoosesMove(BattlePokemonModel player, BattlePokemonModel aiPokemon);

    }
}