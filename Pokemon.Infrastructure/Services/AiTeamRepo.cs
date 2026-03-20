using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Models.Game;
using Pokemon.ContractDTOs.Interfaces;
using Pokemon.Infrastructure.Interfaces;

namespace Pokemon.Infrastructure.Services
{
    public class AiTeamRepo : IAiTeamRepo
    {
        private readonly string _folder;
        private readonly IMauiStorageDirectoryHelper _provider;
        public AiTeamRepo(IMauiStorageDirectoryHelper provier)
        {
            _provider = provier;
            _folder = Path.Combine(_provider.GetDirectory(), "JsonData", "ai_teams");
            Directory.CreateDirectory(_folder);
        }
        public async Task<List<AiTeam>> GetTeams()
        {
            var teams = new List<AiTeam>();

            foreach (var file in Directory.GetFiles(_folder, "*.json"))
            {
                var json = await File.ReadAllTextAsync(file);
                var team = JsonSerializer.Deserialize<AiTeam>(json);
                if (team != null)
                {
                    teams.Add(team);
                }
            }

            return teams;
        }
        public async Task SaveTeam(AiTeam aiTeamPokemon)
        {
            //Spara ett team
            var randomInt = Random.Shared.Next(0, 100);
            var path = Path.Combine(_folder, $"{randomInt}.json");
            var json = JsonSerializer.Serialize(aiTeamPokemon, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(path, json);
        }

    }
}
