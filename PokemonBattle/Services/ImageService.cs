using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Models;
using Pokemon.Services.Interfaces;
using Microsoft.VisualBasic;

namespace PokemonBattle.Services
{
    public class ImageService : IImageService
    {
        private readonly string _baseFolderPath;
        private readonly HttpClient _client;
        public ImageService()
        {
            //MAui storage och inte visual basic storage
            _baseFolderPath = Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, "PokemonSprites");
            Directory.CreateDirectory(_baseFolderPath);
            _client = new HttpClient();
        }

        //hämta mapp genom att gå till pokemon sprites + t.ex. magmar
        public string GetFolder(string name)
        {
            var folder = Path.Combine(_baseFolderPath, name);
            Directory.CreateDirectory(folder);
            Console.WriteLine(folder);
            return folder;
        }
        //hämta specifik sprite genom pokemonsprites/magmar/front_default
        public string GetSprite(string name, string spriteFileName)
        {
            return Path.Combine(GetFolder(name), spriteFileName);
        }
        public Task<SpriteCollection> GetImage(string name)
        {
            throw new NotImplementedException();
        }
        

        public async Task SaveImage(string name, SpriteModel spriteModel)
        {
            //är de viktiga spritesen null?
            if (spriteModel?.GameVersions?.Gen3Sprites?.FireRedLeafGreenSprites == null)
            {
                return;
            }

            //referens till sub sub sub klassen med spritsen
            var frlg = spriteModel.GameVersions.Gen3Sprites.FireRedLeafGreenSprites;

            //sortera innan det blir lokala filer
            var spriteData = new Dictionary<string, string>
            {
                { "front_default.png", frlg.FrontDefault },
                {"front_shiny.png", frlg.FrontShiny },
                {"back_default.png", frlg.BackDefault },
                {"back_shiny.png", frlg.BackShiny },
            };
            foreach(var key in spriteData)
            {
                //tom sträng

                if (string.IsNullOrEmpty(key.Value))
                {
                    continue;
                }
                //Kolla om fil och sökväg finns
                //Key blir front_default
                //Value är själva nät addressen
                var filePath = GetSpritePath(name, key.Key);

                //Den filen finns
                if(File.Exists(filePath))
                {
                    continue;
                }

                using var stream = await _client.GetStreamAsync(key.Value);
                using var fileSteam = File.Create(filePath);
                await stream.CopyToAsync(fileSteam);
            }
        }

        public string GetSpritePath(string pokemonName, string spriteFileName)
        {
            return Path.Combine(GetFolder(pokemonName), spriteFileName);
        }
    }
}
