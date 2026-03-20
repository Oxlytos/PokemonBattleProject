using Domain.Models.Game;
using Pokemon.ContractDTOs.RequestModel;

namespace Pokemon.ContractDTOs.Interfaces
{
    public interface ITypeDataLoader
    {
        Task<List<RequestTypeModel>> LoadTypesFromJsonFolderAsync();
    }
}