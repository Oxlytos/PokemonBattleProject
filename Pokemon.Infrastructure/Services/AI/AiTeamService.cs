using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Models.Game;
using Pokemon.Infrastructure.Interfaces.AI;
using Pokemon.Infrastructure.Models;
using Pokemon.Repository.Services;
using PokemonBattle.Interfaces;

namespace Pokemon.Infrastructure.Services.AI
{
    public class AiTeamService : IAiTeamService
    {
        private string _folder;
        private DirectoryHelperServic _directoryHelperServic;
        public AiTeamService(DirectoryHelperServic provider)
        {
            _directoryHelperServic = provider;
            _folder = Path.Combine(_directoryHelperServic.GetDirectory(), "JsonData", "ai_teams");
            Directory.CreateDirectory(_folder);
        }
      
        public async Task<List<AiTeam>> GetTeams()
        {
            //Hitta alla teams
            var teams = new List<AiTeam>();
            foreach(var file in Directory.GetFiles(_folder, "*.json"))
            {
                var json = await File.ReadAllTextAsync(file);
                var team = JsonSerializer.Deserialize<AiTeam>(json);
                if(team != null)
                {
                    teams.Add(team);
                }
            }
            return teams;

        }

        public async Task SaveTeam(AiTeam aiTeamPokemon)
        {
            //Spara ett team
            var randomInt = Random.Shared.Next(0,100);
            var path = Path.Combine(_folder, $"{randomInt}.json");
            var json = JsonSerializer.Serialize(aiTeamPokemon, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(path, json);
        }
    }
}
