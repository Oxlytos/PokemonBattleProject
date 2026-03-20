using System.Text.Json;
using Domain.Models.Base;
using Domain.Models.Game;
using Pokemon.Infrastructure.Interfaces;

namespace Pokemon.Infrastructure.Services;

    public class JsonStorage : IJsonStorage
    {
        private readonly string _dataFolderPath;
        private IMauiStorageDirectoryHelper _directoryHelperServic;
        public JsonStorage(IMauiStorageDirectoryHelper provider)
        {
            _directoryHelperServic = provider;
            _dataFolderPath = Path.Combine(_directoryHelperServic.GetDirectory(), "JsonData");
            Directory.CreateDirectory(_dataFolderPath);
            Directory.CreateDirectory(Path.Combine(_dataFolderPath, "moves"));
            Directory.CreateDirectory(Path.Combine(_dataFolderPath, "types"));
            Directory.CreateDirectory(Path.Combine(_dataFolderPath, "pokemon"));
        }
        public async Task SaveTeamAsync(List<PartyPokemonModel> team)
        {
            foreach (var item in team)
            {
                Console.WriteLine(item.Nickname);
            }
            var json = JsonSerializer.Serialize(team, new JsonSerializerOptions { WriteIndented = true });
            var filePath = Path.Combine(_dataFolderPath, "team.json");
            await File.WriteAllTextAsync(filePath, json);
        }
        public async Task<List<PartyPokemonModel>> LoadTeamAsync()
        {
            var filePath = Path.Combine(_dataFolderPath, "team.json");
            if (!File.Exists(filePath))
            {
                //tomt på något sätt?
                return new List<PartyPokemonModel>();
            }
            var json = await File.ReadAllTextAsync(filePath);
            var team = JsonSerializer.Deserialize<List<PartyPokemonModel>>(json) ?? new List<PartyPokemonModel>();
            return team;
        }
        public string GetDataFolder(string folderName)
        {
            var folder = Path.Combine(_dataFolderPath, folderName);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            return folder;
        }
        public string? GetMove(string moveName)
        {
            var move = Path.Combine(_dataFolderPath, "moves", moveName + ".json");
            if (!File.Exists(move))
            {
            }
            return move;
        }
        public string? GetTypeM(string typeName)
        {
            var type = Path.Combine(_dataFolderPath, "types", typeName + ".json");
            if (!File.Exists(type))
            {
            }
            return type;
        }
        public async Task<string> GetTypeFolder(string typeName)
        {
            var folder = Path.Combine(_dataFolderPath, "types", typeName);
            //Directory.CreateDirectory(folder);
            return folder;
        }
        public string? GetPokemon(string pokemonName)
        {
            var pokemon = Path.Combine(_dataFolderPath, "pokemon", pokemonName + ".json");
            if (!File.Exists(pokemon))
            {

            }
            return pokemon;
        }
        public async Task SaveMoveData(string moveName, string jsonData)
        {
            if (string.IsNullOrEmpty(moveName))
            {
                return;
            }
            if (string.IsNullOrEmpty(jsonData))
            {
                return;
            }

            var movePath = Path.Combine(_dataFolderPath, "moves", moveName + ".json");
            await File.WriteAllTextAsync(movePath, jsonData);
        }
        public async Task SavePokemonData(string pokemonName, string jsonData)
        {
            if (string.IsNullOrEmpty(pokemonName))
            {
                return;
            }
            if (string.IsNullOrEmpty(jsonData))
            {
                return;
            }

            var movePath = Path.Combine(_dataFolderPath, "pokemon", pokemonName + ".json");
            await File.WriteAllTextAsync(movePath, jsonData);
        }
        public async Task SaveTypeData(string typeName, string jsonData)
        {
            Console.WriteLine(typeName);
            if (string.IsNullOrEmpty(typeName))
            {
                return;
            }
            if (string.IsNullOrEmpty(jsonData))
            {
                return;
            }
            var typePath = Path.Combine(_dataFolderPath, "types", typeName + ".json");
            Console.WriteLine(typePath);
            await File.WriteAllTextAsync(typePath, jsonData);
        }

       
    }
