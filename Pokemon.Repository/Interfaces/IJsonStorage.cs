using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Base;
using Domain.Models.Game;

namespace Pokemon.Repository.Interfaces
{
    public interface IJsonStorage
    {

        //Hitta mapp
        string GetDataFolder(string folderName);

        //Hitta json fil om attack
        string? GetMove(string moveName);
        //Hitta JSON pokemon data
        string? GetPokemon(string pokemonName);

        string? GetTypeM(string typeName);
        //Info om typer
        Task<string> GetTypeFolder(string typeName);
        //Hämta enklaste möjliga domain/base pokemon som UI kan använda
        Task<List<PartyPokemonModel>> LoadTeamAsync();
        //spara attack efter namn + content
        Task SaveMoveData(string moveName, string jsonData);
        //spara pokemon efter namn + content
        Task SavePokemonData(string pokemonName, string jsonData);
        //spara spelares party wuheyeyyy PARTY!
        Task SaveTeamAsync(List<PartyPokemonModel> team);
        //Spara typ information
        Task SaveTypeData(string typeName, string jsonData);
    }
}
