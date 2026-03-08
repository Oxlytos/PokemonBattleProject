using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Models;

namespace Pokemon.Services.Interfaces
{
    public interface IImageService
    {
        public Task<SpriteCollection> GetImage(string name);
        public Task SaveImage(string name, SpriteModel spriteModel);
        public Task<TypeSpriteCollection> GetTypeSprite(string name);
        public Task<string> GetTypeSpriteFolder(string name);
        public Task SaveTypeSprite (string name, TypeSpriteCollection spriteModel);
        string GetSprite(string name, string spriteFileName);
        string GetFolder(string name);
        public Task<string> GetSpritePath(string pokemonName, string spriteFileName);
        public bool AreAllSpritesStored(string pokemonName);


    }
}
