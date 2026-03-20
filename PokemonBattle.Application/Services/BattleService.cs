using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Pokemon.AppServices.Interfaces;

namespace Pokemon.AppServices.Services
{
    public class BattleService : IBattleService
    {
        private ITeamPokemonService _teamPokemonService;
        public List<PartyPokemonModel> playerPartyPokemon = new List<PartyPokemonModel>();
        private readonly Dictionary<double, string> damageAndValuePairs = new Dictionary<double, string> 
        {
            //Expandera om det finns intresse för hypotetisk 8x damage multiplier eller kiknande
            {0, "Had no effect..."},
            {0.25, "Quite inneffective..." },
            {0.5, "Not very effective..."},
            {1, "It was of standard effect" },
            {2, "It was super effective!" },
            {4, "It was ULTRA effective!" }
        
        };

        public BattleService(ITeamPokemonService teamPokemonService)
        {
            _teamPokemonService = teamPokemonService;
            LoadPartyPokemon();
        }
        public async Task<PartyPokemonModel> GetFirstPartyPokemon()
        {
            if (playerPartyPokemon.Count == 0)
            {
                return null;
            }
            return playerPartyPokemon.First();
        }
        public async Task LoadPartyPokemon()
        {
            playerPartyPokemon = _teamPokemonService.TeamPokemon.ToList();
        }

        public async Task<string> GetEffectivnessStatus(double damageMultiplier)
        {

            if (damageAndValuePairs.ContainsKey(damageMultiplier))
            {
                return damageAndValuePairs[damageMultiplier];
            }
            return "";

        }

        public async Task<int> GetAccuracyCheck()
        {
            int accuracy = Random.Shared.Next(0, 100);
            return accuracy;
        }
        public async Task<bool> GetCritChange()
        {
            int thresholdForCrit = 8;
            int crit = Random.Shared.Next(0, 9);
            if(crit < thresholdForCrit)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<double> GetCritModifier()
        {
            double modifier = 1.5;
            return modifier;
        }

        public async Task<double> GetDamageRoll()
        {
            double roll = Random.Shared.NextDouble();
            if (roll < 0.6)
            {
                roll = 0.6;
            }
            return roll;
        }
    }
}
