using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Models.Game;
using Pokemon.AppServices.Interfaces;
using Pokemon.AppServices.Interfaces.AI;

namespace Pokemon.AppServices.Services.AI
{
    public class AiTeamService : IAiTeamService
    {
        private readonly IAiTeamRepo _repo; 
        public AiTeamService(IAiTeamRepo repo)
        {
            _repo = repo;
        }
      
        public async Task<List<AiTeam>> GetTeams()
        {
            return await _repo.GetTeams();
        }

        public async Task SaveTeam(AiTeam aiTeamPokemon)
        {
            await _repo.SaveTeam(aiTeamPokemon);
        }
    }
}
