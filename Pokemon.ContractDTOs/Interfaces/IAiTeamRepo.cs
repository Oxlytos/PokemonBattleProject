using Domain.Models.Game;

namespace Pokemon.AppServices.Interfaces
{
    public interface IAiTeamRepo
    {
        Task<List<AiTeam>> GetTeams();
        Task SaveTeam(AiTeam aiTeamPokemon);
    }
}