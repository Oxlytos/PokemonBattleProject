using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Domain.Models.RequestModels;
using Pokemon.Infrastructure.Factories;
using Pokemon.Infrastructure.Interfaces;
using Pokemon.Infrastructure.Services;
using Pokemon.Services.Interfaces;
using Pokemon.Services.Services;
using Pokemon.Shared.Extensions;
using PokemonBattle.Factories;
using PokemonBattle.ListModel;

namespace PokemonBattle.Facades
{
    public class MoveFacade
    {
        private readonly MoveModelFactory _moveModelFactory;
        private readonly ITeamPokemonService _teamPokemonService;
        private readonly IPokemonFetchRepository _pokemonFetchRepository;
        private readonly IMoveService _moveService;
        private readonly IImageService _imageService;
        private readonly ListMoveModelFactory _listMoveModelFactory;
        public MoveFacade
            (ITeamPokemonService teamPokemonService, 
            IPokemonFetchRepository pokemonFetchRepository, 
            IMoveService moveService, 
            IImageService imageService,
            ListMoveModelFactory listMoveModelFactory,
            MoveModelFactory moveModelFactory
            
            )
        {
            _moveModelFactory = moveModelFactory;
            _listMoveModelFactory = listMoveModelFactory;
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
            ObservableCollection<ListMoveDisplayModel> listMoves = new ObservableCollection<ListMoveDisplayModel>();

            return await _listMoveModelFactory.CreateList(moves);
            
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


            var actualMove = await _moveModelFactory.Create(currentMove);

            pokemon.Moves = await _moveService.AddMove(pokemon, actualMove);

            return await _listMoveModelFactory.Create(actualMove);

        }

        public async Task <ObservableCollection<RequestMoveModel>>? GetAvailableMoves(string name)
        {
            //request modellen
            var pokemonData = await _pokemonFetchRepository.GetPokemonModel(name);

           

            //request
            var currentMoves = pokemonData.LearnedMoves ?? new RequestMoveModel[4];
            //ritkiga moves som blir display sen
            List<MoveModel> moves = await _moveModelFactory.CreateList(currentMoves);
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
