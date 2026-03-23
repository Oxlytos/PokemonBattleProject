using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Domain.Services;
using Pokemon.AppServices.Factories;
using Pokemon.AppServices.Interfaces;
using Pokemon.ContractDTOs.Interfaces;
using Pokemon.ContractDTOs.RequestModel;
using Pokemon.Infrastructure.Interfaces;
using Pokemon.Infrastructure.Services;
using Pokemon.Shared.Extensions;
using PokemonBattle.CollectionViewModels;
using PokemonBattle.Factories;
using PokemonBattle.ListModel;

namespace PokemonBattle.Facades
{
    public class MoveFacade
    {
        private readonly MoveModelFactory _moveModelFactory;
        private readonly ITeamPokemonService _teamPokemonService;
        private readonly IFetchService _pokemonFetchRepository;
        private readonly IMoveService _moveService;
        private readonly IImageService _imageService;
        private readonly ListMoveModelFactory _listMoveModelFactory;
        private readonly ITypeModelFactory _typeModelFactory;
        private readonly TypeDataService _typeDataService;
        private readonly ListStatDisplayFactory _listStatDisplayFactory;
        public MoveFacade
            (ITeamPokemonService teamPokemonService,
            ListStatDisplayFactory listStatDisplayFactory,
            IFetchService pokemonFetchRepository, 
            IMoveService moveService, 
            IImageService imageService,
            ITypeModelFactory typeModelFactory,
            TypeDataService typeDataService,
            ListMoveModelFactory listMoveModelFactory,
            MoveModelFactory moveModelFactory
            
            )
        {
            _listStatDisplayFactory = listStatDisplayFactory;
            _listMoveModelFactory = listMoveModelFactory;
            _teamPokemonService = teamPokemonService;
            _typeDataService = typeDataService;
            _typeModelFactory = typeModelFactory;
            _moveModelFactory = moveModelFactory;
            _listMoveModelFactory = listMoveModelFactory;
            _imageService = imageService;
            _teamPokemonService = teamPokemonService;
            _pokemonFetchRepository = pokemonFetchRepository;
            _moveService = moveService;
        }
        //Find move to remoev
        public async Task<PartyPokemonModel> RemoveMoveFromPokemon(Domain.Models.Game.PartyPokemonModel actualPokemon, ListMoveDisplayModel move)
        {
            Console.WriteLine("Removing move");

            var thisMove = actualPokemon.Moves.FirstOrDefault(x => x.ToLower() == move.Name.ToLower());
            actualPokemon.Moves.Remove(thisMove);

            var thisMoveModel = actualPokemon.Moves.FirstOrDefault(x => x.ToLower() == move.Name.ToLower());
            actualPokemon.Moves.Remove(thisMoveModel);
            _teamPokemonService.UpdateTeamMember(actualPokemon);
            return actualPokemon;
        }

        //return UI list
        public async Task<ObservableCollection<ListMoveDisplayModel>>? UpdateCurrentMovesDisplay(List<string> moves)
        {
            //Lil use of Task.WhenAll here, we get all types of moves, and add to game data in type
            var theseMoveModels = await Task.WhenAll(moves.Select(e => _pokemonFetchRepository.GetSerialisedMoveModelAsync(e)));
            foreach (var moveModel in theseMoveModels)
            {
                var reqMove = await _pokemonFetchRepository.GetMoveModelAsync(moveModel.Name);
                var typeInfo = await _pokemonFetchRepository.GetTypeModelAsync(reqMove.MoveTypeInfo.Name);
                var type = _typeModelFactory.Create(typeInfo);
                moveModel.Type = type;
                _typeDataService.AddType(type);
            }
            return await _listMoveModelFactory.CreateList(theseMoveModels.ToList());
            
        }

        //Max 4 moves, not the same
        public async Task<ListMoveDisplayModel?> AddMoveAsync(RequestMoveModel currentMove, PartyPokemonModel pokemon)
        {
            var canWe = await _moveService.CanWeAddAMove(pokemon);
            if (!canWe)
            {
                return null;
            }
            if (pokemon.Moves.Any(e => e.ToLower() == currentMove.Move.Name.ToLower()))
            {
                return null;
            }
            //With how the request models are retrieved, we get move model, get type, the type data from there
            var reqMove = await _pokemonFetchRepository.GetMoveModelAsync(currentMove.Move.Name);
            var typeInfo = await _pokemonFetchRepository.GetTypeModelAsync(reqMove.MoveTypeInfo.Name);
            var type = _typeModelFactory.Create(typeInfo);
            _typeDataService.AddType(type);

            //Get proper movemodel here, all relevant data is available
            var actualMove = await _pokemonFetchRepository.GetSerialisedMoveModelAsync(reqMove.Move.Name);
            actualMove.Type = type;
            pokemon.Moves.Add(actualMove.Name);
            _teamPokemonService.UpdateTeamMember(pokemon);

            var listMOve = await _listMoveModelFactory.Create(actualMove);

            return listMOve;

        }

        public async Task <ObservableCollection<RequestMoveModel>>? GetAvailableMoves(string name)
        {
            //request model
            var pokemonData = await _pokemonFetchRepository.GetPokemonSingularAsync(name);

            //avialble learned moves or empty array of 4 moves that can be assigned
            var currentMoves = pokemonData.LearnedMoves ?? new RequestMoveModel[4];


            //ritkiga moves som blir display sen
            foreach (var move in currentMoves)
            {
                move.Move.Name = move.Move.Name.Capitalize();
                var actualMove = await _pokemonFetchRepository.GetSerialisedMoveModelAsync(move.Move.Name);
            }


            var movesList = pokemonData.Moves?.Moves ?? new RequestMoveModel[1];
            foreach (var move in movesList)
            {
                move.Move.Name = move.Move.Name.Capitalize();
            }

            var returnMovset = new ObservableCollection<RequestMoveModel>(movesList);
            return returnMovset;
        }

        public Task<string> GetPokemonSpriteAsyncPNG(string name, string version)
        {
            return _imageService.GetPokemonSpriteAsyncPNG(name, version);
        }

        public async Task<ListStatDisplayModel> GetDisplayBaseStats(StatModel stats)
        {
            return await _listStatDisplayFactory.Create(stats);
        }
    }
}
