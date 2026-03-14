using Domain.Models.Game;

namespace Pokemon.Repository.Interfaces
{
    public interface ITypeDataLoader
    {
        Task LoadTypesFromJsonFolderAsync();
        public Task AddTypeModel(TypeModel newType);
    }
}