using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using PokemonBattle.CollectionViewModels;

namespace PokemonBattle.Factories
{
    public class ListStatDisplayFactory
    {
        public async Task<ListStatDisplayModel> Create(StatModel model)
        {
            var listStast = new ListStatDisplayModel();

            listStast.DisplayHealth = "Health: " + model.BaseHealth;
            listStast.DisplayAttack = "Attack: " + model.BaseAttack;
            listStast.DisplayDefense = "Defense: " + model.BaseDefense;
            listStast.DisplaySpecialAttack = "Special Attack: " + model.BaseSpecialAttack;
            listStast.DisplaySpecialDefense = "Special Defense: " + model.BaseSpecialAttack;
            listStast.DisplaySpeed = "Speed: " + model.BaseSpeed;
            return listStast;
        }
    }
}
