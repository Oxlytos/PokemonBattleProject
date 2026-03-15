using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Pokemon.Infrastructure.Models;

namespace Pokemon.Infrastructure.Interfaces.AI
{
    public interface IAiTeamService
    {
        Task<List<AiTeam>> GetTeams();
        Task SaveTeam(AiTeam aiTeamPokemon);
    }
}
