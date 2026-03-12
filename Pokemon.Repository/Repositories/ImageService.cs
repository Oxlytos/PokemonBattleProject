using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pokemon.Services.Interfaces;
using Microsoft.VisualBasic;
using System.IO;
using System.Net.NetworkInformation;
using Domain.Models.RequestModels;
using System.Text.Json;
using Pokemon.Repository.Interfaces;
using System.Reflection.Metadata;
using System.Xml.Linq;
using Domain.Models.Game;
using PokemonBattle.Interfaces;

namespace PokemonBattle.Services
{
    public class ImageService : IImageService
    {
        private readonly string _baseFolderPath;
        private readonly string _typeFolderPath;
        private readonly HttpClient _client;
        private readonly string _baseUrl = "https://pokeapi.co/api/v2/";
        private readonly IMauiStorageDirectoryHelper _provider;
        public ImageService(IMauiStorageDirectoryHelper provider)
        {
            //MAui storage och inte visual basic storage
            _provider = provider;
            _baseFolderPath = Path.Combine(_provider.GetDirectory(), "PokemonSprites");
            _typeFolderPath = Path.Combine(_provider.GetDirectory(), "TypeSprites");
            Directory.CreateDirectory(_baseFolderPath);
            Directory.CreateDirectory(_typeFolderPath);
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_baseUrl);
        }

        //hämta mapp genom att gå till pokemon sprites + t.ex. magmar
        public string GetFolder(string name)
        {
            var folder = Path.Combine(_baseFolderPath, name);
            if (Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            return folder;
        }
        public string GetTypeFolder(string name)
        {
            var folder = Path.Combine(_typeFolderPath, name);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            return folder;
        }
        //hämta specifik sprite genom pokemonsprites/magmar/front_default
        public string GetSprite(string name, string spriteFileName)
        {
            return Path.Combine(GetFolder(name), spriteFileName);
        }
        public async Task SaveImage(string name, RequestSpriteModel spriteModel)
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
            foreach (var key in spriteData)
            {
                //tom sträng

                if (string.IsNullOrEmpty(key.Value))
                {
                    //continue;
                }
                //Kolla om fil och sökväg finns
                //Key blir front_default
                //Value är själva nät addressen
                var filePath = await GetSpritePath(name, key.Key);

                //Kolla om map finns, på key.key som är front_defaul t.ex.
                var folder = Path.GetDirectoryName(filePath);



                //finns inte ens mappen
                if (!Directory.Exists(folder))
                {
                    //skapa mappen
                    Directory.CreateDirectory(folder);
                }

                //Vi behöver ladda ner om X fil inte finns, eller om det är tomt
                bool needToDownloadAgain = !File.Exists(filePath) || new FileInfo(filePath).Length == 0;

                if (!needToDownloadAgain)
                {
                    continue;
                }

                if (needToDownloadAgain)
                {
                    using var stream = await _client.GetStreamAsync(key.Value);
                    using var fileSteam = File.Create(filePath);
                    await stream.CopyToAsync(fileSteam);
                }

            }
        }

        public async Task<string> GetSpritePath(string pokemonName, string spriteFileName)
        {
            var folder = GetFolder(pokemonName);
            return Path.Combine(folder, spriteFileName);
        }

        public bool AreAllSpritesStored(string pokemonName)
        {
            //Hitta mapp
            string folder = GetFolder(pokemonName);

            //Är dessa här`??
            string[] spriteFileNames = { "front_default.png", "front_shiny.png", "back_default.png", "back_shiny.png" };

            //Loopa igenom och kolla att det finns en väg till dem
            foreach (var sprite in spriteFileNames)
            {
                string existingPath = Path.Combine(folder, sprite);
                if (!File.Exists(existingPath) || new FileInfo(existingPath).Length == 0)
                {
                    return false;
                }
            }
            return true;
        }
        public async Task<string> GetTypeSpriteFolder(string name)
        {
            var folder = GetTypeFolder(name);
            return Path.Combine(folder, name+".png");
        }
        public async Task<string[]> GetTypeSprite(string[] typeNames)
        {
            if (typeNames.Length==0|| typeNames == null)
            {
                return Array.Empty<string>();
            }
            //lagra vägarna till filerna här
            var paths = new List<string>();
            foreach(var type in typeNames)
            {
                if (string.IsNullOrEmpty(type))
                {
                    continue;
                }
                var thisLocalFile = Path.Combine(_typeFolderPath, type, $"{type}.png");

                if (!File.Exists(thisLocalFile))
                {
                    try
                    {


                        string request = "type/" + type;
                        HttpResponseMessage msg = await _client.GetAsync(request);
                        if (!msg.IsSuccessStatusCode)
                        {
                            return Array.Empty<string>();
                        }
                        else
                        {
                            var content = await msg.Content.ReadAsStringAsync();

                            var typedata = JsonSerializer.Deserialize<RequestTypeModel>(content);
                            Console.WriteLine(typedata);
                            if (typedata == null)
                            {
                                return Array.Empty<string>();
                            }
                            await SaveTypeSprite(type, typedata.Sprites);
                            //Retunera 1 fil, en array av filer (2) eller inget
                        }
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
                if (File.Exists(thisLocalFile))
                {
                    paths.Add(thisLocalFile);
                }
                
            }
            return paths.ToArray();
        }


        public async Task SaveTypeSprite(string name, TypeSpriteCollection spriteModel)
        {
            if(spriteModel == null)
            {
                return;
            }

            var spriteUrl = spriteModel.TypeCollections.FireRedLeafGreenTypeIconSprite.TypeIconUrl;

            if (string.IsNullOrEmpty(spriteUrl))
            {
                return;
            }


            //Kolla om map finns, på key.key som är front_defaul t.ex.
            var folder = Path.Combine(_typeFolderPath,name);
            //skapa mappen
            Directory.CreateDirectory(folder);
            var filePath = Path.Combine(folder, $"{name}.png");
            using var stream = await _client.GetStreamAsync(spriteUrl);
            using var fileSteam = File.Create(filePath);
            await stream.CopyToAsync(fileSteam);

        }

        public async Task<string> GetPokemonSpriteAsyncPNG(string name, string version = "front_default")
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            var file = Path.Combine(_baseFolderPath,name, version+".png");
            if (File.Exists(file)&&AreAllSpritesStored(name))
            {
                return file;
              
            }
           
            try
            {
                string request = "pokemon/" + name;
                HttpResponseMessage msg = await _client.GetAsync(request);
                if (msg == null)
                {
                    return null;
                }
                var content = await msg.Content.ReadAsStringAsync();
                var sprites = JsonSerializer.Deserialize<RequestSpriteModel>(content);
                if (sprites != null)
                {
                    await SaveImage(name, sprites);

                }

                return File.Exists(file) ? file : null;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }
    }
}
