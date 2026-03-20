using System.Text.Json;
using Domain.Models.Game;
using Domain.Services;
using Pokemon.ContractDTOs.RequestModel;
using Pokemon.Infrastructure.Interfaces;

namespace Pokemon.Infrastructure.Services
{
    public class TypeDataJsonLoader : ITypeDataLoader
    {
        private IMauiStorageDirectoryHelper _directoryHelperServic;
        string _dataFolder;
        public TypeDataJsonLoader(TypeDataService typeDataService, IMauiStorageDirectoryHelper directoryHelperServic)
        {
            _directoryHelperServic = directoryHelperServic;
            var baseDir = _directoryHelperServic.GetDirectory();
            _dataFolder = Path.Combine(baseDir, "JsonData");
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
    }
}
