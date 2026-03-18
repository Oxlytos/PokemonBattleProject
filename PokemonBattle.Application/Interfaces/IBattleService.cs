using Domain.Models.Game;

namespace Pokemon.Infrastructure.Interfaces
{
    public interface IBattleService
    {
        Task<PartyPokemonModel> GetFirstPartyPokemon();
        Task LoadPartyPokemon();
        Task<string> GetEffectivnessStatus(double damageMultiplier);
        Task<int> GetAccuracyCheck();
        Task<bool> GetCritChange();
        Task<double> GetCritModifier();
        Task<double> GetDamageRoll();
    }
}