using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.RequestModels;

namespace Pokemon.Repository.Interfaces
{
        public interface IJsonStorage
        {
            string GetDataFolder(string folderName);
            string? GetMove(string moveName);
            string? GetPokemon(string pokemonName);
            Task<string> GetTypeFolder(string typeName);
            Task<List<RequestPokeonModel>> LoadTeamAsync();
            Task SaveMoveData(string moveName, string jsonData);
            Task SavePokemonData(string pokemonName, string jsonData);
            Task SaveTeamAsync(List<RequestPokeonModel> team);
            Task SaveTypeData(string typeName, string jsonData);
        }
}
