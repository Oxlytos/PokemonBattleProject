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
        string GetSprite(string name, string spriteFileName);
        string GetFolder(string name);
        public string GetSpritePath(string pokemonName, string spriteFileName);


    }
}
