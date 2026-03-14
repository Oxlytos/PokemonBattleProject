using Domain.Models.Game;

namespace Pokemon.Infrastructure.Interfaces
{
    public interface IAIService
    {
        Task<MoveModel> AIChoosesMove();

    }
}