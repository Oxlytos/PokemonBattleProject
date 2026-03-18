using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Domain.Models.RequestModels;
using Pokemon.AppServices.Factories;
using Pokemon.Infrastructure.Interfaces;
using Pokemon.Infrastructure.Interfaces.AI;
using Pokemon.Infrastructure.Models;
using Pokemon.Infrastructure.Services;
using Pokemon.Repository.Interfaces;
using Pokemon.Services.Interfaces;
using PokemonBattle.Interfaces;
using PokemonBattle.ListModel;
using PokemonBattle.ViewModels;

namespace PokemonBattle.Facades
{
    public class UIFacade
    {
        private IPokemonFetchService _fetchService;
        private IImageService _imageService;
        private ITeamPokemonService _teamPokemonService;
        private IJsonStorage _jsonStorage;
        private IMoveService _moveService;
        private IAiTeamService _aiTeamService;
        private readonly ListPokemonDisplayModelFactory _displayModelFactory;
        private readonly PartyPokemonFactory _partyPokemonFactory;

        public UIFacade(
            IPokemonFetchService pokemonFetchService,
            IImageService imageService,
            ITeamPokemonService teamPokemonService,
            ITypeService typeService,
            IMauiStorageDirectoryHelper mauiStorageDirectoryHelper,
            IJsonStorage jsonStorage,
            IMoveService moveService,
            IAiTeamService aiTeamService,
            PartyPokemonFactory partyPokemonFactory,
            //IBattleService battleService,
            ListPokemonDisplayModelFactory displayModelFactory)
        {
            _partyPokemonFactory = partyPokemonFactory;
            _aiTeamService = aiTeamService;
            //_battleService = battleService;
            _moveService = moveService;
            _fetchService = pokemonFetchService;
            _imageService = imageService;
            _teamPokemonService = teamPokemonService;
            _jsonStorage = jsonStorage;
            _displayModelFactory = displayModelFactory;

        }

        public async Task<PartyPokemonModel?> GetPartyPokemon(ListPokemonDisplayModel clickedThisItem, ObservableCollection<ListPokemonDisplayModel> currentTeam)
        {
            if (!currentTeam.Contains(clickedThisItem))
            {
                return null;
            }
            int index = currentTeam.IndexOf(clickedThisItem);
            var thisPokemon = _teamPokemonService.TeamPokemon[index];

            return thisPokemon;
        }
        public async Task<int?> RemoveFromPartyAndUITeam(ListPokemonDisplayModel pokeom, ObservableCollection<ListPokemonDisplayModel> currentTeam )
        {
            //Är denna listitem faktiskst i listan
            if (!currentTeam.Contains(pokeom))
            {
                return null;
            }
            
            //Hitta dess index i listan
            int index = currentTeam.IndexOf(pokeom);

            Console.WriteLine(_teamPokemonService.TeamPokemon);
            //Se till att party pokemon går bort också
            var correspondingPartyPokeon = _teamPokemonService.TeamPokemon[index];
            await _teamPokemonService.RemoveFromTeam(correspondingPartyPokeon);
            //Ge index till UI som den tar bort
            return index;

        }
        public async Task<List<PartyPokemonModel>> GetPokemonTeamAsync()
        {
            var team = _teamPokemonService.TeamPokemon;
            return team.ToList();
        }
        public async Task<List<RequestPokeonModel>> GetAllPokemonAsync()
        {
            var pokemon = await _fetchService.GetPokemonAsync();
            return pokemon;
        }
        public async Task LoadTeamAsync()
        {
            var team = await _jsonStorage.LoadTeamAsync();
            if (team != null)
            {
                _teamPokemonService.TeamPokemon.Clear();
                foreach (var pokemon in team)
                {
                    Console.WriteLine(pokemon.Nickname);
                    _teamPokemonService.TeamPokemon.Add(pokemon);
                }
            }

        }
        public async Task SaveTeamForAI()
        {
            var aiTeam = _teamPokemonService.TeamPokemon.ToList();
            if (aiTeam == null && aiTeam.Count == 0)
            {
                return;
            }


            AiTeam aisNewTeam = new AiTeam
            {
                Name = DateTime.Now.ToString(),
                AiPokemon = aiTeam
            };


            if (aisNewTeam != null && aisNewTeam.AiPokemon.Count > 0)
            {
                await _aiTeamService.SaveTeam(aisNewTeam);

            }
        }
        public async Task SaveTeam()
        {
            
            await _jsonStorage.SaveTeamAsync(_teamPokemonService.TeamPokemon.ToList());
        }
        public async Task<string?> LoadPokemonFrontSpritePathAsync(string name)
        {
            //vägen dit (om den finns)
            var path = await _imageService.GetPokemonSpriteAsyncPNG(name);

            //finns inte
            if (!_imageService.AreAllSpritesStored(name))
            {

                var fullPokemonInfo = await _fetchService.GetPokemonSingularAsync(name);
                await _imageService.SaveImage(name, fullPokemonInfo.Sprites.SpriteModel);

            }
            return path;
        }
        public async Task<string?> LoadPokemonBackSpritePathAsync(string name)
        {
            //vägen dit (om den finns)
            var path = await _imageService.GetPokemonBackSpriteAsyncPNG(name);

            //finns inte
            if (!_imageService.AreAllSpritesStored(name))
            {

                var fullPokemonInfo = await _fetchService.GetPokemonSingularAsync(name);
                await _imageService.SaveImage(name, fullPokemonInfo.Sprites.SpriteModel);

            }
            return path;
        }
        public async Task<string[]?> LoadTypeSpritePaths(string pokemonName)
        {
            var pokemon = await _fetchService.GetPokemonSingularAsync(pokemonName);

            var typeNames = pokemon.Types.Select(t => t.Types.Name).ToArray();

            var typePaths = (await _imageService.GetTypeSprite(typeNames)).ToArray();
            return typePaths;
        }
        public async Task<ListPokemonDisplayModel?> AddToUITeam(RequestPokeonModel newMember)
        {
            if(!await _teamPokemonService.CanWeAddToTeam())
            {
                return null;
            }
            Console.WriteLine(newMember.Nickname);
            newMember = await _fetchService.GetPokemonSingularAsync(newMember.Name);
            Console.WriteLine(newMember.Nickname);
            var partyPokemon = _partyPokemonFactory.Create(newMember);

            var displayPokemon = await _displayModelFactory.CreateFrontFacingSprite(partyPokemon);
            Console.WriteLine(displayPokemon.Nickname);
            return displayPokemon;


        }
        public async Task<PartyPokemonModel?> AddToPartyTeam(RequestPokeonModel newMember)
        {
            if(!await _teamPokemonService.CanWeAddToTeam())
            {
                return null;
            }
             newMember = await _fetchService.GetPokemonSingularAsync(newMember.Name);
            var partyPokemon = _partyPokemonFactory.Create(newMember);
            Console.WriteLine(newMember.Nickname);
            await _teamPokemonService.AddToTeam(partyPokemon);
            return partyPokemon;
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
            if (pokemon.Moves.Any(e=>e.Name.ToLower()==currentMove.Move.Name.ToLower()))
            {
                return null;
            }

            var requestMove = await _fetchService.GetMoveModelAsync(currentMove.Move.Name);

            var actualMove = await _fetchService.GetSerialisedMoveModelAsync(requestMove.Move.Name);

            var typeData = await _fetchService.GetTypeModelAsync(requestMove.MoveTypeInfo.Name);

            pokemon.Moves = await _moveService.AddMove(pokemon, actualMove);

            ListMoveDisplayModel move = new ListMoveDisplayModel(actualMove);
            move.Power = requestMove.Power != null ? (int?)requestMove.Power : null;
            move.TypeName = requestMove.MoveTypeInfo.Name;

            return move;

        }
        public async Task<bool> CanUserGoToBattlePage()
        {
            if(_teamPokemonService.TeamPokemon.Count == 0)
            {
                return false;
            }

            return true;
        }
    }
}
