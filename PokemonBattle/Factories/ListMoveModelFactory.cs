using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Pokemon.Infrastructure.Interfaces;
using Pokemon.Infrastructure.Repositories;
using Pokemon.Services.Interfaces;
using Pokemon.Shared.Extensions;
using PokemonBattle.ListModel;

namespace PokemonBattle.Factories
{
    public class ListMoveModelFactory
    {
        private IFetchService _pokemonFetchService;
        public ListMoveModelFactory(IFetchService pokemonFetchService)
        {
            _pokemonFetchService = pokemonFetchService;
        }
        public async Task<ListMoveDisplayModel> Create(MoveModel basemove)
        {
            ListMoveDisplayModel newMove = new ListMoveDisplayModel(basemove);
            if (!string.IsNullOrEmpty(newMove.Name))
            {
                var typeInfo = await _pokemonFetchService.GetMoveModelAsync(newMove.Name);
                newMove.TypeName = typeInfo.MoveTypeInfo.Name.Capitalize();
                newMove.Name = newMove.Name.Capitalize();
            }
            return newMove;
        }
        public async Task<ObservableCollection<ListMoveDisplayModel>> CreateList(List<MoveModel> moves)
        {
            ObservableCollection<ListMoveDisplayModel> listMoves = new ObservableCollection<ListMoveDisplayModel>();
            foreach (var move in moves)
            {
                ListMoveDisplayModel newMove = new ListMoveDisplayModel(move);
                if (!string.IsNullOrEmpty(newMove.Name))
                {
                    var typeInfo = await _pokemonFetchService.GetMoveModelAsync(newMove.Name);
                    newMove.TypeName = typeInfo.MoveTypeInfo.Name.Capitalize();
                    newMove.Name = newMove.Name.Capitalize();
                }
                listMoves.Add(newMove);
            }

            return listMoves;
        }
    }
}
