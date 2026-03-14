using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Models.Game;
using Domain.Services;
using Pokemon.Repository.Interfaces;
using PokemonBattle.Interfaces;

namespace Pokemon.Repository.Repositories
{
    public class TypeDataLoader : ITypeDataLoader
    {
        private readonly TypeDataService _typeDataService;
        IMauiStorageDirectoryHelper _provider;
        string _dataFolder;
        public TypeDataLoader(TypeDataService typeDataService, IMauiStorageDirectoryHelper mauiStorageDirectoryHelper)
        {
            _typeDataService = typeDataService;
            _provider = mauiStorageDirectoryHelper;
            _dataFolder = Path.Combine(_provider.GetDirectory(), "JsonData", "types");

        }
        public async Task LoadTypesFromJsonFolderAsync()
        {
            foreach (var file in Directory.GetFiles(_dataFolder))
            {
                var json = await File.ReadAllTextAsync(file);
                var type = JsonSerializer.Deserialize<TypeModel>(json);
                if (type == null)
                {
                    continue;
                }
                _typeDataService.AddType(type);
            }
        }
        public async Task AddTypeModel(TypeModel newType)
        {
            _typeDataService.AddType(newType);
        }
    }
}
