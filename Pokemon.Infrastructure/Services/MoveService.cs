using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Models.Models;
using Pokemon.Infrastructure.Interfaces;

namespace Pokemon.Infrastructure.Services
{
 
    public class MoveService : IMoveService
    {
        public async Task<MoveRequestModel[]> AddMove(PokemonModel pokemon, MoveRequestModel newMove)
        {
            var moves = pokemon.LearnedMoves?.ToList() ?? new List<MoveRequestModel>();
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
        public async Task<MoveRequestModel[]> RemoveMove(PokemonModel pokemon, MoveRequestModel move)
        {
            var moves = pokemon.LearnedMoves.ToList();
            moves.Remove(move);
            pokemon.LearnedMoves = moves.ToArray();
            return pokemon.LearnedMoves;
        }

    }
}
