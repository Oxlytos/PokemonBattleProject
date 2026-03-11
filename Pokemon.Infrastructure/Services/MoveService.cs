using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.RequestModels;
using Pokemon.Infrastructure.Interfaces;

namespace Pokemon.Infrastructure.Services
{
 
    public class MoveService : IMoveService
    {
        public async Task<RequestMoveModel[]> AddMove(RequestPokeonModel pokemon, RequestMoveModel newMove)
        {
            var moves = pokemon.LearnedMoves?.ToList() ?? new List<RequestMoveModel>();
            if (moves.Count >= 4)
            {
                return pokemon.LearnedMoves;
            }
            if (!moves.Any(e=>e.Move.Name==newMove.Move.Name))
            {
               
                moves.Add(newMove);
                pokemon.LearnedMoves = moves.ToArray();
            }
            return pokemon.LearnedMoves;
        }
        public async Task<RequestMoveModel[]> RemoveMove(RequestPokeonModel pokemon, RequestMoveModel move)
        {
            var moves = pokemon.LearnedMoves.ToList();
            moves.Remove(move);
            pokemon.LearnedMoves = moves.ToArray();
            return pokemon.LearnedMoves;
        }

    }
}
