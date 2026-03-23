using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Pokemon.AppServices.Factories;
using Pokemon.AppServices.Interfaces;
using Pokemon.ContractDTOs.Interfaces;
using Pokemon.ContractDTOs.RequestModel;

namespace Pokemon.AppServices.Services
{
 
    public class MoveService : IMoveService
    {
        private readonly IFetchRepository _fetchRepository;
        private ITypeModelFactory _typeModelFactory;
        private MoveModelFactory _moveModelFactory;
        public MoveService(MoveModelFactory moveModelFactory, IFetchRepository fetchRepository, ITypeModelFactory typeModelFactory)
        {
            _moveModelFactory = moveModelFactory;
            _fetchRepository = fetchRepository;
            _typeModelFactory = typeModelFactory;
        }
        //Add move ifs not the same
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
        //basic check
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
        //

        public async Task<List<string>> AddMove(PartyPokemonModel pokemon, MoveModel newMove)
        {
            var currentMoves = pokemon.Moves?.ToList() ?? new List<string>();
            if(currentMoves.Count >= 4)
            {
                return currentMoves;
            }

            if (!currentMoves.Contains(newMove.Name))
            {
                currentMoves.Add(newMove.Name);
            }
            return currentMoves;
        }

        public async Task<List<string>> RemoveMove(PartyPokemonModel pokemon, MoveModel move)
        {
            string toRemove = move.Name;
            pokemon.Moves.Remove(toRemove);
            return pokemon.Moves;
        }


        public async Task<RequestMoveModel[]> RemoveMove(RequestPokeonModel pokemon, RequestMoveModel move)
        {
            var moves = pokemon.LearnedMoves.ToList();
            moves.Remove(move);
            pokemon.LearnedMoves = moves.ToArray();
            return pokemon.LearnedMoves;
        }

        public async Task<List<MoveModel>> GetMoveModels(List<string> moves)
        {
            //Get all movemodels for input moves
            var moveModels = await Task.WhenAll(moves.Select(e => _fetchRepository.GetSerialisedMoveModelAsync(e)));

            //Handles typing the moves
            foreach (var moveModel in moveModels)
            {
                var basicType = await _fetchRepository.GetMoveModelAsync(moveModel.Name);
                var data = await _fetchRepository.GetTypeModelAsync(basicType.MoveTypeInfo.Name);
                var typeData = _typeModelFactory.Create(data);
                moveModel.Type = typeData;
            }
            return moveModels.ToList();
        }
    }
}
