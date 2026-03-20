using Domain.Models.Game;

namespace Pokemon.ContractDTOs.Interfaces
{
    public interface IAiTeamRepo
    {
        Task<List<AiTeam>> GetTeams();
        Task SaveTeam(AiTeam aiTeamPokemon);
    }
}