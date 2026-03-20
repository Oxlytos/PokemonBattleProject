using Domain.Models.Game;
using Pokemon.ContractDTOs.RequestModel;

namespace Pokemon.Infrastructure.Interfaces
{
    public interface ITypeDataLoader
    {
        Task<List<RequestTypeModel>> LoadTypesFromJsonFolderAsync();
    }
}