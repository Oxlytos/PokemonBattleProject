using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Models.Models;
using Pokemon.Infrastructure.Interfaces;

namespace Pokemon.Infrastructure.Repositories
{
    public class TypeRepo : ITypeRepo
    {
        private readonly HttpClient _client;
        private string baseUrl = "https://pokeapi.co/api/v2/";

        public TypeRepo(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new Uri(baseUrl);
        }
        public async Task<TypeModel> GetTypeData(string name)
        {
            string request = "type/" + name;

            HttpResponseMessage msg = await _client.GetAsync(request);

            if (msg.IsSuccessStatusCode)
            {
                var content = await msg.Content.ReadAsStringAsync();

                var typedata = JsonSerializer.Deserialize<TypeModel>(content);
                Console.WriteLine(typedata);

                return typedata;

            }

            return null;
        }

        public async Task<string> GetTypeSprite(string name)
        {
            string request = "type/" + name;

            HttpResponseMessage msg = await _client.GetAsync(request);

            if (msg.IsSuccessStatusCode)
            {
                var content = await msg.Content.ReadAsStringAsync();

                var typedata = JsonSerializer.Deserialize<TypeModel>(content);

                string typeUrl = typedata.Sprites.TypeCollections.FireRedLeafGreenTypeIconSprite.TypeIconUrl;
                Console.WriteLine(typeUrl);

            }

            return null;
        }

        public Task SaveTypeData(string name)
        {
            throw new NotImplementedException();
        }
    }
}
