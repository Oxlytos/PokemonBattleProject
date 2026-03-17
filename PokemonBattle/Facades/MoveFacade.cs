using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Domain.Models.RequestModels;
using Pokemon.Infrastructure.Interfaces;
using Pokemon.Infrastructure.Services;
using Pokemon.Services.Interfaces;
using Pokemon.Services.Services;
using Pokemon.Shared.Extensions;
using PokemonBattle.ListModel;

namespace PokemonBattle.Facades
{
    public class MoveFacade
    {
        private readonly ITeamPokemonService _teamPokemonService;
        private readonly IPokemonFetchRepository _pokemonFetchRepository;
        private readonly IMoveService _moveService;
        private readonly IImageService _imageService;
        public MoveFacade(ITeamPokemonService teamPokemonService, IPokemonFetchRepository pokemonFetchRepository, IMoveService moveService, IImageService imageService)
        {
            _imageService = imageService;
            _teamPokemonService = teamPokemonService;
            _pokemonFetchRepository = pokemonFetchRepository;
            _moveService = moveService;
        }
        public async Task<PartyPokemonModel> RemoveMoveFromPokemon(Domain.Models.Game.PartyPokemonModel actualPokemon, ListMoveDisplayModel move)
        {
            Console.WriteLine("Removing move");

            var thisMove = actualPokemon.Moves.FirstOrDefault(x => x.Name.ToLower() == move.Name.ToLower());
            actualPokemon.Moves.Remove(thisMove);

            var thisMoveModel = actualPokemon.Moves.FirstOrDefault(x => x.Name.ToLower() == move.Name.ToLower());
            actualPokemon.Moves.Remove(thisMoveModel);
            _teamPokemonService.UpdateTeamMember(actualPokemon);
            return actualPokemon;
        }

        public async Task<ObservableCollection<ListMoveDisplayModel>>? UpdateCurrentMovesDisplay(List<MoveModel> moves)
        {
            List<ListMoveDisplayModel> listMoves = new List<ListMoveDisplayModel>();
            foreach (var move in moves)
            {
                ListMoveDisplayModel newMove = new ListMoveDisplayModel(move);
                if (!string.IsNullOrEmpty(newMove.Name))
                {
                    var typeInfo = await _pokemonFetchRepository.GetMoveModelAsync(newMove.Name);
                    newMove.TypeName = typeInfo.MoveTypeInfo.Name.Capitalize();
                    newMove.Name = newMove.Name.Capitalize();
                }
                listMoves.Add(newMove);
            }

            var movesToReturn = new ObservableCollection<ListMoveDisplayModel>(listMoves);
            foreach (var move in movesToReturn)
            {
                move.Name = move.Name.Capitalize();
                Console.WriteLine(move.Name);
            }
            foreach (var move in movesToReturn)
            {
                Console.WriteLine(move.Name);
                throw new NotImplementedException();
            }

            return movesToReturn;
        }
        public async Task<ListMoveDisplayModel?> AddMoveAsync(RequestMoveModel currentMove, PartyPokemonModel pokemon)
        {

            var canWe = await _moveService.CanWeAddAMove(pokemon);
            //kolla om det finns 4 moves redan
            if (!canWe)
            {
                return null;
            }
            //inte samma move 4 gånger
            if (pokemon.Moves.Any(e => e.Name.ToLower() == currentMove.Move.Name.ToLower()))
            {
                return null;
            }

            var requestMove = await _pokemonFetchRepository.GetMoveModelAsync(currentMove.Move.Name);

            var actualMove = await _pokemonFetchRepository.GetSerialisedMoveModelAsync(requestMove.Move.Name);

            var typeData = await _pokemonFetchRepository.GetTypeModelAsync(requestMove.MoveTypeInfo.Name);

            pokemon.Moves = await _moveService.AddMove(pokemon, actualMove);

            ListMoveDisplayModel move = new ListMoveDisplayModel(actualMove);
            move.Power = requestMove.Power != null ? (int?)requestMove.Power : null;
            move.TypeName = requestMove.MoveTypeInfo.Name;

            return move;

        }

        public async Task <ObservableCollection<RequestMoveModel>>? GetAvailableMoves(string name)
        {
            //request modellen
            var pokemonData = await _pokemonFetchRepository.GetPokemonModelModelAsync(name);

            //ritkiga moves som blir display sen
            List<MoveModel> moves = new List<MoveModel>();

            //request
            var currentMoves = pokemonData.LearnedMoves ?? new RequestMoveModel[4];
            foreach (var move in currentMoves)
            {
                move.Move.Name = move.Move.Name.Capitalize();
                var actualMove = await _pokemonFetchRepository.GetSerialisedMoveModelAsync(move.Move.Name);
                moves.Add(actualMove);
            }


            var movesList = pokemonData.Moves?.Moves ?? new RequestMoveModel[1];
            foreach (var move in movesList)
            {
                move.Move.Name = move.Move.Name.Capitalize();
            }

            var returnMovset = new ObservableCollection<RequestMoveModel>(movesList);
            return returnMovset;
        }

        public Task<string> GetPokemonSpriteAsyncPNG(string name)
        {
            return _imageService.GetPokemonSpriteAsyncPNG(name);
        }
    }
}
