using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Pokemon.Infrastructure.Models;

namespace Pokemon.Infrastructure.Factories
{
    public static class BattlePokemonFactory
    {
        public static BattlePokemonModel Create(PartyPokemonModel partyPokemonModel)
        {
            BattlePokemonModel battle = new BattlePokemonModel(partyPokemonModel);
            return battle;
        }
        public static List<BattlePokemonModel> CreateBattleTeam(List<PartyPokemonModel> team)
        {
            List<BattlePokemonModel> battleTeam = new List<BattlePokemonModel>();
            foreach (PartyPokemonModel party in team)
            {
                battleTeam.Add(new BattlePokemonModel(party));
            }
            return battleTeam;
        }
    }
}
