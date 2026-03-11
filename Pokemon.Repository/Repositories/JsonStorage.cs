using System.Text.Json;
using Domain.Models.Base;
using Domain.Models.Game;
using Pokemon.Repository.Interfaces;
using PokemonBattle.Interfaces;

namespace Pokemon.Repository.Repositories
{
  

    public class JsonStorage : IJsonStorage
    {
        HttpClient _client;
        private readonly string _dataFolderPath;
        IMauiStorageDirectoryHelper _provider;
        public JsonStorage(IMauiStorageDirectoryHelper provider)
        {
            _provider = provider;
            _dataFolderPath = Path.Combine(_provider.GetDirectory(), "JsonData");
            Directory.CreateDirectory(_dataFolderPath);
            Directory.CreateDirectory(Path.Combine(_dataFolderPath, "moves"));
            Directory.CreateDirectory(Path.Combine(_dataFolderPath, "types"));
            Directory.CreateDirectory(Path.Combine(_dataFolderPath, "pokemon"));
            _client = new HttpClient();
        }
        public async Task SaveTeamAsync(List<PartyPokemonModel> team)
        {
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
        public async Task<string> GetTypeFolder(string typeName)
        {
            var folder = Path.Combine(_dataFolderPath, "types", typeName);
            Directory.CreateDirectory(folder);
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
            if (string.IsNullOrEmpty(typeName))
            {
                return;
            }
            if (string.IsNullOrEmpty(jsonData))
            {
                return;
            }
            var filepath = await GetTypeFolder(typeName);

            var movePath = Path.Combine(filepath, jsonData);
            await File.WriteAllTextAsync(movePath, jsonData);
        }

    }
}
