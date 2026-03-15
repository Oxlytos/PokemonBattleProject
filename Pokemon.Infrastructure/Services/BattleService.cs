using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Pokemon.Infrastructure.Interfaces;
using Pokemon.Repository.Interfaces;

namespace Pokemon.Infrastructure.Services
{
    public class BattleService : IBattleService
    {
        public List<PartyPokemonModel> playerPartyPokemon = new List<PartyPokemonModel>();

        private readonly IJsonStorage _jsonStorage;
        public BattleService(IJsonStorage jsonStorage)
        {
            _jsonStorage = jsonStorage;
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
            playerPartyPokemon = await _jsonStorage.LoadTeamAsync();
        }


        public Task PlayerMove()
        {
            throw new NotImplementedException();
        }

        public Task AIMove()
        {
            throw new NotImplementedException();
        }
    }
}
