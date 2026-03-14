using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Models.Base;
using Domain.Models.Game;
using Domain.Models.RequestModels;
using Pokemon.Infrastructure.Factories;
using Pokemon.Infrastructure.Interfaces;
using Pokemon.Repository.Interfaces;
using Pokemon.Repository.Repositories;

namespace Pokemon.Infrastructure.Repositories
{
    public class PokemonFetchRepository : IPokemonFetchRepository
    {
        private readonly HttpClient _client;
        private readonly IJsonStorage _jsonStorage;
        private readonly ITypeDataLoader _typeDataLoader;
        private string baseUrl = "https://pokeapi.co/api/v2/";
        public PokemonFetchRepository(HttpClient client, IJsonStorage jsonStorage, ITypeDataLoader typeDataLoader)
        {
            _client = client;
            _client.BaseAddress = new Uri(baseUrl);
            _jsonStorage = jsonStorage;
            _typeDataLoader = typeDataLoader;
        }

        public async Task<RequestPokeonModel> DeserializePokemonModel(string jsonResponse)
        {
            var normalPokemon = JsonSerializer.Deserialize<RequestPokeonModel>(jsonResponse);
            normalPokemon.Name = char.ToUpper(normalPokemon.Name[0]) + normalPokemon.Name.Substring(1);
            var sprites = JsonSerializer.Deserialize<SpriteCollection>(jsonResponse);
            normalPokemon.Sprites = sprites;
            var pokemonMoves = JsonSerializer.Deserialize<MoveRequestCollection>(jsonResponse);
            Console.WriteLine(pokemonMoves);
            normalPokemon.Moves = pokemonMoves;
            Console.WriteLine(normalPokemon.Sprites);
            //Gör till den fånigt långa och komplicerade spritecollectionen
            return normalPokemon;

        }

        public async Task<RequestMoveModel> DeserializeMoveModel(string jsonResponse)
        {
            var move = JsonSerializer.Deserialize<RequestMoveModel>(jsonResponse);
            var moveInfo = JsonSerializer.Deserialize<Move>(jsonResponse);
            Console.WriteLine(moveInfo);
            move.Move = moveInfo;

            var moveType = JsonSerializer.Deserialize<RequestTypeModel>(jsonResponse);
            //var typemodel = TypeModelFactory.Create(moveType);
            //Console.WriteLine(typemodel);
            //await _typeDataLoader.AddTypeModel(typemodel);
            return move;
        }



        public async Task<RequestTypeModel> DeserializeTypeModel(string jsonResponse)
        {
            var typeJson = JsonSerializer.Deserialize<RequestTypeModel>(jsonResponse);
            var typemodel = TypeModelFactory.Create(typeJson);
            Console.WriteLine(typemodel);
            await _typeDataLoader.AddTypeModel(typemodel);
            return typeJson;
        }

        public async Task<RequestMoveModel> GetMoveModelAsync(string name)
        {
            var localFileCheck = _jsonStorage.GetMove(name);
            if (localFileCheck != null && File.Exists(localFileCheck))
            {
                //detta är filen, läs den istället för att ladda ner igen
                var jsonContent = await File.ReadAllTextAsync(localFileCheck);
                var result = await DeserializeMoveModel(jsonContent);
                return result;
            }

            string request = "move/" + name;

            HttpResponseMessage msg = await _client.GetAsync(request);
            Console.WriteLine("Status code" + msg.StatusCode);
            try
            {
                if (msg.IsSuccessStatusCode)
                {
                    var content = await msg.Content.ReadAsStringAsync();
                    Console.WriteLine(content);
                    var move = JsonSerializer.Deserialize<RequestMoveModel>(content);

                    var moveInfo = JsonSerializer.Deserialize<Move>(content);
                    var moveType = JsonSerializer.Deserialize<MoveType>(content);
                    Console.WriteLine(moveInfo.Name);
                    move.Move = moveInfo;
                    Console.WriteLine(move.Move.Name);

                    Console.WriteLine(move);
                    await _jsonStorage.SaveMoveData(name, content);
                    return move;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Nothing happended?");
            return null;
        }

        public async Task<RequestPokeonModel> GetPokemonModelModelAsync(string name)
        {
            //detta är filvägen
            var localFileCheck = _jsonStorage.GetPokemon(name);
            if (localFileCheck != null && File.Exists(localFileCheck))
            {
                //detta är filen, läs den istället för att ladda ner igen
                var jsonContent = await File.ReadAllTextAsync(localFileCheck);
                var result = await DeserializePokemonModel(jsonContent);
                return result;
            }
            //har vi inte redan infomrationen, ladda ner
            //requestlänk för att hämta pokemon och bilder
            string request = "pokemon/" + name;

            //Hämta
            HttpResponseMessage msg = await _client.GetAsync(request);

            try
            {
                if (msg.IsSuccessStatusCode)
                {
                    //Läs till sträng
                    var content = await msg.Content.ReadAsStringAsync();
                    var normalPokemon = await DeserializePokemonModel(content);
                    Console.WriteLine(normalPokemon);

                    normalPokemon.Name = char.ToUpper(normalPokemon.Name[0]) + normalPokemon.Name.Substring(1);
                    var sprites = JsonSerializer.Deserialize<SpriteCollection>(content);
                    normalPokemon.Sprites = sprites;
                    var pokemonMoves = JsonSerializer.Deserialize<MoveRequestCollection>(content);
                    normalPokemon.Moves = pokemonMoves;
                    Console.WriteLine(normalPokemon);

                    var typeTsks = normalPokemon.Types.Select(e=>GetTypeModelAsync(e.Types.Name)).ToList();
                    await Task.WhenAll(typeTsks);

                    await _jsonStorage.SavePokemonData(name, content);
                    return normalPokemon;

                }
                else
                {

                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
                
            Console.WriteLine("Nothing happended?");
            return null;

        }

        public async Task<List<RequestPokeonModel>> GetPokemonModelsAsync()
        {
            HttpResponseMessage responsemsg = await _client.GetAsync(_client.BaseAddress + "pokemon?limit=151&offset=0");
            if (responsemsg.IsSuccessStatusCode)
            {
                var pokeJson = await responsemsg.Content.ReadFromJsonAsync<RequestPokemonListModel>();
                var pokemonPlural = pokeJson.Result.ToList();
                List<RequestPokeonModel> result = new List<RequestPokeonModel>();
                foreach (var pokemon in pokemonPlural)
                {
                    pokemon.Name = char.ToUpper(pokemon.Name[0]) + pokemon.Name.Substring(1);
                    result.Add(pokemon);
                }
                return result;
            }
            else
            {
                return null;
            }
        }

        public async Task<RequestTypeModel> GetTypeModelAsync(string name)
        {
            var localFileCheck = Path.Combine(await _jsonStorage.GetTypeFolder(name), name + ".json");
            if (localFileCheck != null && File.Exists(localFileCheck))
            {
                return await DeserializeTypeModel(localFileCheck);
            }
            //requestlänk för att hämta pokemon och bilder
            string request = "type/" + name;

            //Hämta
            HttpResponseMessage msg = await _client.GetAsync(_client.BaseAddress + request);

            try
            {
                if (msg.IsSuccessStatusCode)
                {
                    string content = await msg.Content.ReadAsStringAsync();
                    var typeJson = await DeserializeTypeModel(content);
                    //await _typeDataLoader.add
                    await _jsonStorage.SaveTypeData(name, content);
                    var typemodel = TypeModelFactory.Create(typeJson);
                    Console.WriteLine(typemodel);
                    await _typeDataLoader.AddTypeModel(typemodel);
                    return typeJson;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;

        }

        public async Task<BasePokemon> GetBasePokemonDeseralized(string jsonContet)
        {
            var poke = JsonSerializer.Deserialize<BasePokemon>(jsonContet);
            return poke;
        }
        public async Task<MoveModel> GetMoveModelDeseralized(string jsonContet)
        {
            var move = JsonSerializer.Deserialize<MoveModel>(jsonContet);
            return move;
        }

        public async Task<TypeModel> GetTypeModelDeseralized(string jsonContet)
        {
            var type = JsonSerializer.Deserialize<TypeModel>(jsonContet);
            return type;
        }

        public async Task<BasePokemon> GetBasePokemonAsync(string name)
        {
            var localFileCheck = _jsonStorage.GetPokemon(name);
            if (localFileCheck == null && !File.Exists(localFileCheck))
            {
                return null;
            }
            //detta är filen, läs den istället för att ladda ner igen
            var jsonContent = await File.ReadAllTextAsync(localFileCheck);
            var result = await GetBasePokemonDeseralized(jsonContent);
            return result;
        }

        public async Task<MoveModel> GetSerialisedMoveModelAsync(string name)
        {
            var localFileCheck = _jsonStorage.GetMove(name);
            if (localFileCheck == null && !File.Exists(localFileCheck))
            {
                return null;
            }
            //detta är filen, läs den istället för att ladda ner igen
            var jsonContent = await File.ReadAllTextAsync(localFileCheck);
            var result = await GetMoveModelDeseralized(jsonContent);
            return result;
        }

        public async Task<TypeModel> GetSerialisedTypeModelAsync(string name)
        {
            var localFileCheck = _jsonStorage.GetPokemon(name);
            if (localFileCheck == null && !File.Exists(localFileCheck))
            {
                return null;
            }
            //detta är filen, läs den istället för att ladda ner igen
            var jsonContent = await File.ReadAllTextAsync(localFileCheck);
            var result = await GetTypeModelDeseralized(jsonContent);
            return result;
        }
    }
}
