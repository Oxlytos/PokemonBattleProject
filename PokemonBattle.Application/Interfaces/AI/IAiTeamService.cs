using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;

namespace Pokemon.AppServices.Interfaces.AI
{
    public interface IAiTeamService
    {
        Task<List<AiTeam>> GetTeams();
        Task SaveTeam(AiTeam aiTeamPokemon);
    }
}
