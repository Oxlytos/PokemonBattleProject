using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.RequestModels;

namespace Pokemon.Services.Interfaces
{
    public interface IImageService
    {
        public Task<SpriteCollection> GetImage(string name);
        public Task SaveImage(string name, RequestSpriteModel spriteModel);
        public Task<string[]> GetTypeSprite(string[] name);
        public Task<string> GetTypeSpriteFolder(string name);
        public Task SaveTypeSprite (string name, TypeSpriteCollection spriteModel);
        Task<string> GetPokemonSpriteAsyncPNG(string name, string version = "front_default");
        string GetSprite(string name, string spriteFileName ="front_default");
        string GetFolder(string name);
        public Task<string> GetSpritePath(string pokemonName, string spriteFileName);
        public bool AreAllSpritesStored(string pokemonName);


    }
}
