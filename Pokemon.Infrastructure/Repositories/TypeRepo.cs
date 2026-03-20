using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Pokemon.ContractDTOs.RequestModel;
using Pokemon.Infrastructure.Interfaces;

namespace Pokemon.Infrastructure.Repositories
{
    public class TypeRepo : ITypeRepo
    {
        private readonly HttpClient _client;
        private string baseUrl = "https://pokeapi.co/api/v2/";
        private IJsonStorage _storage;

        public TypeRepo(HttpClient client, IJsonStorage storage)
        {
            _client = client;
            _client.BaseAddress = new Uri(baseUrl);
            _storage = storage;
        }
        public async Task<RequestTypeModel> DeseralizeTypeInfo(string content)
        {
            var typedata =  JsonSerializer.Deserialize<RequestTypeModel>(content);
            return typedata;
        }
        public async Task<RequestTypeModel> GetTypeData(string name)
        {
            var localFileCheck = await _storage.GetTypeFolder(name);
            if(localFileCheck!=null && File.Exists(localFileCheck))
            {
                var data = await DeseralizeTypeInfo(localFileCheck);
                return data;
            }
            string request = "type/" + name;

            HttpResponseMessage msg = await _client.GetAsync(request);

            if (msg.IsSuccessStatusCode)
            {
                var content = await msg.Content.ReadAsStringAsync();

                var typedata = await DeseralizeTypeInfo(content);
                await SaveTypeData(typedata);
                return typedata;

            }
            return null;
        }

        public Task<RequestTypeModel> GetTypes()
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetTypeSprite(string name)
        {

            string request = "type/" + name;

            HttpResponseMessage msg = await _client.GetAsync(request);

            if (msg.IsSuccessStatusCode)
            {
                var content = await msg.Content.ReadAsStringAsync();

                var typedata = JsonSerializer.Deserialize<RequestTypeModel>(content);

                string typeUrl = typedata.Sprites.TypeCollections.FireRedLeafGreenTypeIconSprite.TypeIconUrl;
                Console.WriteLine(typeUrl);

            }

            return null;
        }

        public async Task SaveTypeData(RequestTypeModel requestType)
        {
            await _storage.SaveTypeData(requestType.Name, requestType.ToString());
        }
    }
}
