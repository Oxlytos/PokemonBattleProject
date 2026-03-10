using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Models.Models;
using Pokemon.Infrastructure.Interfaces;
using Pokemon.Repository.Interfaces;
using Pokemon.Repository.Repositories;

namespace Pokemon.Infrastructure.Repositories
{
    public class PokemonFetchRepository : IPokemonFetchRepository
    {
        private readonly HttpClient _client;
        private readonly IJsonStorage _jsonStorage;
        private string baseUrl = "https://pokeapi.co/api/v2/";
        public PokemonFetchRepository(HttpClient client, IJsonStorage jsonStorage)
        {
            _client = client;
            _client.BaseAddress = new Uri(baseUrl);
            _jsonStorage = jsonStorage;
        }

        public async Task<PokemonModel> DeserializePokemonModel(string jsonResponse)
        {
            var normalPokemon = JsonSerializer.Deserialize<PokemonModel>(jsonResponse);
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

        public async Task<MoveModel> DeserializeMoveModel(string jsonResponse)
        {
            var move = JsonSerializer.Deserialize<MoveModel>(jsonResponse);
            var moveType = JsonSerializer.Deserialize<TypeModel>(jsonResponse);
            Console.WriteLine(moveType);
            return move;
        }



        public async Task<TypeModel> DeserializeTypeModel (string jsonResponse)
        {
            var typeJson =  JsonSerializer.Deserialize<TypeModel>(jsonResponse);
            return typeJson;
        }

        public async Task<MoveModel> GetMoveModelAsync(string name)
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
                    
                    var move = JsonSerializer.Deserialize<MoveModel>(content);
                    var moveType = JsonSerializer.Deserialize<TypeRequest>(content);

                    move.Type = moveType;

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

        public async Task<PokemonModel> GetPokemonModelModelAsync(string name)
        {
            //detta är filvägen
            var localFileCheck = _jsonStorage.GetPokemon(name);
            if(localFileCheck !=null && File.Exists(localFileCheck))
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
            Console.WriteLine("Status code" +msg.StatusCode);

            try
            {
                if (msg.IsSuccessStatusCode)
                {
                    //Läs till sträng
                        var content = await msg.Content.ReadAsStringAsync();
                        var normalPokemon = JsonSerializer.Deserialize<PokemonModel>(content);
                        
                        normalPokemon.Name = char.ToUpper(normalPokemon.Name[0]) + normalPokemon.Name.Substring(1);
                        var sprites = JsonSerializer.Deserialize<SpriteCollection>(content);
                        normalPokemon.Sprites = sprites;
                        var pokemonMoves = JsonSerializer.Deserialize<MoveRequestCollection>(content);
                        normalPokemon.Moves = pokemonMoves;
                        Console.WriteLine(normalPokemon.Sprites);
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

        public async Task<List<PokemonModel>> GetPokemonModelsAsync()
        {
            HttpResponseMessage responsemsg = await _client.GetAsync(_client.BaseAddress+ "pokemon?limit=151&offset=0");
            if (responsemsg.IsSuccessStatusCode)
            {
                var pokeJson = await responsemsg.Content.ReadFromJsonAsync<PokemonListRequestModel>();
                var pokemonPlural = pokeJson.Result.ToList();
                List<PokemonModel> result = new List<PokemonModel>();
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

        public async Task<TypeModel> GetTypeModelAsync(string name)
        {
            var localFileCheck = await _jsonStorage.GetTypeFolder(name);
            if (localFileCheck !=null && File.Exists(localFileCheck))
            {
                return await DeserializeTypeModel(localFileCheck);
            }
            //requestlänk för att hämta pokemon och bilder
            string request = "type/" + name;

            //Hämta
            HttpResponseMessage msg = await _client.GetAsync(_client.BaseAddress+request);

            try
            {
                if (msg.IsSuccessStatusCode)
                {
                    string content = await msg.Content.ReadAsStringAsync();
                    var typeJson = await DeserializeTypeModel(content);

                     await _jsonStorage.SaveTypeData(name, content);
                    return typeJson;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;

        }
    }
}
