using Domain.Models.Game;
using Domain.Models.RequestModels;

namespace Pokemon.Repository.Interfaces
{
    public interface ITypeDataLoader
    {
        Task<List<RequestTypeModel>> LoadTypesFromJsonFolderAsync();
        public Task AddTypeModel(RequestTypeModel newType);
    }
}