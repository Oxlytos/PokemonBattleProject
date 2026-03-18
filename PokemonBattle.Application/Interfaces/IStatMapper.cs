using Domain.Models.Game;
using Domain.Models.RequestModels;

namespace Pokemon.AppServices.Interfaces
{
    public interface IStatMapper
    {
        StatModel MapStats(RequestPokeonModel request);
    }
}