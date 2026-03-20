using Domain.Models.Game;
using Domain.Services;
using Pokemon.ContractDTOs.Interfaces;

namespace Pokemon.AppServices.Services
{
    public class TypeInitalizerService
    {
        //Ladda in i början och lägg till här senare
        private readonly ITypeDataLoader _repo;
        private readonly ITypeModelFactory _modelFactory;
        private readonly TypeDataService _typeDataService;
        public TypeInitalizerService(ITypeDataLoader repo, ITypeModelFactory typeModelFactory, TypeDataService typeDataService)
        {
            _repo = repo;
            _typeDataService = typeDataService;
            _modelFactory = typeModelFactory;
        }
        public async Task InitializeTypesAsync()
        {
            var allStoredTypes = await _repo.LoadTypesFromJsonFolderAsync();
            foreach (var jType in allStoredTypes)
            {
                var domainType = _modelFactory.Create(jType);
                _typeDataService.AddType(domainType);
            }
        }
        public async Task AddTypeToDomain(TypeModel domainType)
        {
            _typeDataService.AddType(domainType);
        }
    }
}
