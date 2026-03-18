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
        string _dataFolder;
        public TypeDataLoader(TypeDataService typeDataService, DirectoryHelperServic directoryHelperServic)
        {
            _typeDataService = typeDataService;
            _directoryHelperServic = directoryHelperServic;
            var baseDir = _directoryHelperServic.GetDirectory();
            _dataFolder = Path.Combine(baseDir, "JsonData");
            //_dataFolder = Path.Combine(_provider.GetDirectory(), "JsonData", "types");
            //_dataFolderPath = Path.Combine(_provider.GetDirectory(), "JsonData");


        }
        public async Task<List<RequestTypeModel>> LoadTypesFromJsonFolderAsync()
        {
            var folder = Path.Combine(_dataFolder, "types");

            Directory.CreateDirectory(folder);

            var result = new List<RequestTypeModel>();

            var files = Directory.GetFiles(folder);
            Console.WriteLine(files.Length);
            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                var type = JsonSerializer.Deserialize<RequestTypeModel>(json);
                if (type != null)
                {
                    result.Add(type);
                }

            }
            return result;

        }
        public async Task AddTypeModel(RequestTypeModel newType)
        {
            _typeDataService.AddType(newType);
        }
    }
}
