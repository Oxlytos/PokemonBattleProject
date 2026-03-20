using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Pokemon.AppServices.Interfaces;
using Pokemon.AppServices.Models;

namespace Pokemon.AppServices.Factories
{
    public class BattlePokemonFactory
    {
        private readonly IMoveService _moveService;
        public BattlePokemonFactory(IMoveService moveService)
        {
            _moveService = moveService;
        }
        public  async Task<BattlePokemonModel>Create(PartyPokemonModel partyPokemonModel)
        {
            BattlePokemonModel battle = new BattlePokemonModel(partyPokemonModel);
            battle.Moves = await _moveService.GetMoveModels(partyPokemonModel.Moves);
            return battle;
        }
        public async Task<List<BattlePokemonModel>> CreateBattleTeam(List<PartyPokemonModel> team)
        {
            List<BattlePokemonModel> battleTeam = new List<BattlePokemonModel>();
            foreach (PartyPokemonModel party in team)
            {
                var battle = new BattlePokemonModel(party);
                battle.Moves = await _moveService.GetMoveModels(party.Moves);
                battleTeam.Add(new BattlePokemonModel(party));
            }
            
            return battleTeam;
        }
    }
}
