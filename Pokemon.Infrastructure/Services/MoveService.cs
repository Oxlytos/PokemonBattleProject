using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
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
        public async Task<bool> CanWeAddAMove(PartyPokemonModel partyPokemonModel)
        {
            if (partyPokemonModel == null)
            {
                return false;
            }
            if(partyPokemonModel.Moves.Count < 4)
            {
                return true;
            }
            return false;
        }

        public async Task<List<MoveModel>> AddMove(PartyPokemonModel pokemon, MoveModel newMove)
        {
            Console.WriteLine(pokemon.Moves);
            var currentMoves = pokemon.Moves?.ToList() ?? new List<MoveModel>();
            if(currentMoves.Count >= 4)
            {
                return currentMoves;
            }

            if (!currentMoves.Contains(newMove))
            {
                currentMoves.Add(newMove);
            }
            return currentMoves;
        }
        public async Task<List<MoveModel>> RemoveMove(PartyPokemonModel pokemon, MoveModel move)
        {
            pokemon.Moves.Remove(move);
            return pokemon.Moves;
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
