using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pokemon.Infrastructure.Interfaces;
using Pokemon.Repository.Interfaces;
using Pokemon.Services.Interfaces;
using PokemonBattle.Interfaces;

namespace Pokemon.AppServices.Facades
{
    //https://refactoring.guru/design-patterns/facade/csharp/example
    public class PokemonFacade
    {
        public PokemonFacade(
             IPokemonFetchService pokemonFetchService,
            IImageService imageService,
            ITeamPokemonService teamPokemonService,
            ITypeService typeService,
            IMauiStorageDirectoryHelper mauiStorageDirectoryHelper,
            IJsonStorage jsonStorage)
        {
            
        }

    }
}
