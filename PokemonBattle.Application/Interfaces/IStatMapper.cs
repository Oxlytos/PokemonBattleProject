using Domain.Models.Game;
using Pokemon.ContractDTOs.RequestModel;

namespace Pokemon.AppServices.Interfaces
{
    public interface IStatMapper
    {
        StatModel MapStats(RequestPokeonModel request);
    }
}