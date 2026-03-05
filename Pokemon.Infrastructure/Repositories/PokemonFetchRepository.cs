using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Models.Models;
using Pokemon.Infrastructure.Interfaces;

namespace Pokemon.Infrastructure.Repositories
{
    public class PokemonFetchRepository : IPokemonFetchRepository
    {
        private readonly HttpClient _client;
        private string baseUrl = "https://pokeapi.co/api/v2/";
        public PokemonFetchRepository(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new Uri(baseUrl);
        }

        public async Task<PokemonModel> GetPokemonModelModelAsync(string name)
        {
          
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

                    //Gör till pokemon model
                    var normalPokemon = JsonSerializer.Deserialize<PokemonModel>(content);

                    //Gör till den fånigt långa och komplicerade spritecollectionen
                    var sprites = JsonSerializer.Deserialize<SpriteCollection>(content);

                    //Pokemonen har nu koll på sina egna sprites (url länkar, inte lokala)
                    normalPokemon.Sprites = sprites;
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
                return pokemonPlural;
            }
            else
            {
                return null;
            }
        }
    }
}
