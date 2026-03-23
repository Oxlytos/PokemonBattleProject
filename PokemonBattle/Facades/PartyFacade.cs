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
using Pokemon.AppServices.Interfaces.AI;
using Pokemon.ContractDTOs.Interfaces;
using Pokemon.ContractDTOs.RequestModel;
using Pokemon.Infrastructure.Interfaces;
using Pokemon.Infrastructure.Services;
using PokemonBattle.Factories;
using PokemonBattle.ListModel;
using PokemonBattle.ViewModels;

namespace PokemonBattle.Facades
{
    public class UIFacade
    {
        private IFetchService _fetchService;
        private IImageService _imageService;
        private ITeamPokemonService _teamPokemonService;
        private IJsonStorage _jsonStorage;
        private IMoveService _moveService;
        private IAiTeamService _aiTeamService;
        private ITypeModelFactory _typeModelFactory;
        private TypeDataService _typeDataService;
        private readonly ListPokemonDisplayModelFactory _displayModelFactory;
        private readonly PartyPokemonFactory _partyPokemonFactory;
        private readonly ListMoveModelFactory _listMoveModelFactory;

        public UIFacade(
            IFetchService pokemonFetchService,
            IImageService imageService,
            ITeamPokemonService teamPokemonService,
            IMauiStorageDirectoryHelper mauiStorageDirectoryHelper,
            IJsonStorage jsonStorage,
            IMoveService moveService,
            IAiTeamService aiTeamService,
            ITypeModelFactory typeModelFactory,
            TypeDataService typeDataService,
            PartyPokemonFactory partyPokemonFactory,
            ListMoveModelFactory listMoveModelFactory,
            ListPokemonDisplayModelFactory displayModelFactory)
        {
            _typeDataService = typeDataService;
            _typeModelFactory = typeModelFactory;
            _listMoveModelFactory = listMoveModelFactory;
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

        //Get current pokemon data by clicking on its image
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
        //remove function, by clicking on the button on the UI
        public async Task<int?> RemoveFromPartyAndUITeam(ListPokemonDisplayModel pokeom, ObservableCollection<ListPokemonDisplayModel> currentTeam )
        {
            if (!currentTeam.Contains(pokeom))
            {
                return null;
            }
            
            //Remove with index
            int index = currentTeam.IndexOf(pokeom);

            Console.WriteLine(_teamPokemonService.TeamPokemon);
            //Remove the same pokemon in the acual team, they share the same index
            var correspondingPartyPokeon = _teamPokemonService.TeamPokemon[index];
            await _teamPokemonService.RemoveFromTeam(correspondingPartyPokeon);
            return index;

        }
        //
        public Task<List<PartyPokemonModel>> GetPokemonTeam()
        {
            var team = _teamPokemonService.TeamPokemon;
            return Task.FromResult(team.ToList());
        }
        //Fetch all 151 first pokemon
        public async Task<List<RequestPokeonModel>> GetAllPokemonAsync()
        {
            var pokemon = await _fetchService.GetPokemonAsync();
            return pokemon;
        }

        //Loads team async from JSON format
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
        //Creates a AI team which can be used, by the AI
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
        //Saves team in JSON format
        public async Task SaveTeam()
        {
            
            await _jsonStorage.SaveTeamAsync(_teamPokemonService.TeamPokemon.ToList());
        }
        //Gets images, and tries and get if they're not all there
        public async Task<string?> LoadPokemonFrontSpritePathAsync(string name)
        {

            //Om det saknas någon bild överhuvudtaget => Ladda ner
            if (!_imageService.AreAllSpritesStored(name))
            {
                var fullPokemonInfo = await _fetchService.GetPokemonSingularAsync(name);
                //Failed, can be retried
                if (fullPokemonInfo?.Sprites?.SpriteModel == null)
                {
                    Console.WriteLine($"Missing sprite data for {name} :((");
                    Console.WriteLine("Next call of method will prorably have it downloaded");
                    return null;
                }

                Console.WriteLine(fullPokemonInfo);
                await _imageService.SaveImage(name, fullPokemonInfo.Sprites.SpriteModel);

            }

            //SMALL pause, downloads are fast, but a small delay enables safety of loading images
            await Task.Delay(50);

            string version = "front_default";

            string path = await _imageService.GetPokemonSpriteAsyncPNG(name, version);

            return path;
        }
        public async Task<string?> LoadPokemonBackSpritePathAsync(string name)
        {
            string version = "back_default";

            var path = await _imageService.GetPokemonBackSpriteAsyncPNG(name, version);

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
        //Adds to ui team
        public async Task<ListPokemonDisplayModel?> AddToUITeam(RequestPokeonModel newMember)
        {
            if(!await _teamPokemonService.CanWeAddToTeam())
            {
                return null;
            }
            newMember = await _fetchService.GetPokemonSingularAsync(newMember.Name);

            //Type images we get from images
            //And also real types we add to game data
            foreach(var type in newMember.Types)
            {
                var requestType = await _fetchService.GetTypeModelAsync(type.Types.Name);
                var properType = _typeModelFactory.Create(requestType);
                _typeDataService.AddType(properType);
            }

            //Create an acutal pokemon from ui image
            //NOT SAVED, just for the displaymodelfactory
            
            var partyPokemon = _partyPokemonFactory.Create(newMember);

            //then just return a ready-for-ui element
            var displayPokemon = await _displayModelFactory.CreateFrontFacingSprite(partyPokemon);
            return displayPokemon;


        }

        //Adds to actual team
        public async Task<PartyPokemonModel?> AddToPartyTeam(RequestPokeonModel newMember)
        {
            if(!await _teamPokemonService.CanWeAddToTeam())
            {
                return null;
            }
            newMember = await _fetchService.GetPokemonSingularAsync(newMember.Name);
            var partyPokemon = _partyPokemonFactory.Create(newMember);
            await _teamPokemonService.AddToTeam(partyPokemon);
            return partyPokemon;
        }

        public async Task<ListMoveDisplayModel?> AddMoveAsync(RequestMoveModel currentMove, PartyPokemonModel pokemon)
        {

            var canWe = await _moveService.CanWeAddAMove(pokemon);
            if (!canWe)
            {
                return null;    
            }
            //Cant have same move, no duplicates allowerd
            if (pokemon.Moves.Any(e=>e.ToLower()==currentMove.Move.Name.ToLower()))
            {
                return null;
            }

            var requestMove = await _fetchService.GetMoveModelAsync(currentMove.Move.Name);

            var actualMove = await _fetchService.GetSerialisedMoveModelAsync(requestMove.Move.Name);

            var typeData = await _fetchService.GetTypeModelAsync(requestMove.MoveTypeInfo.Name);

            pokemon.Moves = await _moveService.AddMove(pokemon, actualMove);

            return await _listMoveModelFactory.Create(actualMove);

        }
        //Stops from loading a battle with no pokemon
        public async Task<bool> CanUserGoToBattlePage()
        {
            if(_teamPokemonService.TeamPokemon.Count == 0)
            {
                return false;
            }
            if (_teamPokemonService.TeamPokemon.Any(P => P.Moves.Count == 0))
            {
                return false;
            }

            return true;
        }
    }
}
