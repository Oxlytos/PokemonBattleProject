using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Pokemon.Services.Interfaces;
using PokemonBattle.ListModel;
using PokemonBattle.Services;

namespace Pokemon.AppServices.Factories
{
    public class ListPokemonDisplayModelFactory
    {
        private readonly IImageService _imageService;

        public ListPokemonDisplayModelFactory(IImageService imageService)
        {
            _imageService = imageService;
        }
        public async Task<ListPokemonDisplayModel> CreateFrontFacingSprite(PartyPokemonModel model)
        {
            var listPoke = new ListPokemonDisplayModel(model);
            listPoke.Types = model.Types.ToArray();
            listPoke.SpritePath = await _imageService.GetPokemonSpriteAsyncPNG(listPoke.Name);
            listPoke.SpriteTypePaths = await _imageService.GetTypeSprite(listPoke.Types);
            return listPoke;

        }
        public async Task<ListPokemonDisplayModel> CreateBackFacingSprite(PartyPokemonModel model)
        {
            var listPoke = new ListPokemonDisplayModel(model);
            listPoke.Types = model.Types.ToArray();
            listPoke.SpritePath = await _imageService.GetPokemonBackSpriteAsyncPNG(listPoke.Name);
            listPoke.SpriteTypePaths = await _imageService.GetTypeSprite(listPoke.Types);
            return listPoke;
        }
    }
}
