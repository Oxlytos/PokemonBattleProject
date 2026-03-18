using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Pokemon.Infrastructure.Models;

namespace Pokemon.Infrastructure.Factories
{
    public class BattlePokemonFactory
    {
        public  BattlePokemonModel Create(PartyPokemonModel partyPokemonModel)
        {
            BattlePokemonModel battle = new BattlePokemonModel(partyPokemonModel);
            return battle;
        }
        public List<BattlePokemonModel> CreateBattleTeam(List<PartyPokemonModel> team)
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
