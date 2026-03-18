using System.Text.Json;
using Domain.Models.Game;
using Domain.Models.RequestModels;
using Domain.Services;
using Pokemon.Repository.Interfaces;
using Pokemon.Repository.Services;
using PokemonBattle.Interfaces;

namespace Pokemon.Repository.Repositories
{
    public class TypeDataLoader : ITypeDataLoader
    {
        private readonly TypeDataService _typeDataService;
        private DirectoryHelperServic _directoryHelperServic;
        ITypeModelFactory _typeModelFactory;
        string _dataFolder;
        public TypeDataLoader(TypeDataService typeDataService, DirectoryHelperServic directoryHelperServic, ITypeModelFactory typeModelFactory)
        {
            _typeDataService = typeDataService;
            _directoryHelperServic = directoryHelperServic;
            var baseDir = _directoryHelperServic.GetDirectory();
            _dataFolder = Path.Combine(baseDir, "JsonData");
            _typeModelFactory = typeModelFactory;
            //_dataFolder = Path.Combine(_provider.GetDirectory(), "JsonData", "types");
            //_dataFolderPath = Path.Combine(_provider.GetDirectory(), "JsonData");


        }
        public async Task LoadTypesFromJsonFolderAsync()
        {
            var folder = Path.Combine(_dataFolder, "types");

            Directory.CreateDirectory(folder);

            try
            {
                var files = Directory.GetFiles(folder);
                Console.WriteLine(files.Length);
                foreach (var file in files)
                {
                    Console.WriteLine(file);
                    var json =  File.ReadAllText(file);
                    var type = JsonSerializer.Deserialize<RequestTypeModel>(json);
                    if (type == null)
                    {
                        continue;
                    }
                    var actualType = _typeModelFactory.Create(type);
                    Console.WriteLine(type);
                    _typeDataService.AddType(actualType);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }
        public async Task AddTypeModel(TypeModel newType)
        {
            _typeDataService.AddType(newType);
        }
    }
}
