using Microsoft.Extensions.Logging;
using Pokemon.Infrastructure.Interfaces;
using Pokemon.Infrastructure.Repositories;
using Pokemon.Services.Interfaces;
using Pokemon.Services.Services;
using PokemonBattle.Interfaces;
using PokemonBattle.ViewModels;
using Pokemon.Infrastructure.Services;
using PokemonBattle.Services;
using Pokemon.Repository.Repositories;
using Pokemon.Repository.Interfaces;
using Pokemon.AppServices.Factories;
using PokemonBattle.Facades;
using Domain.Interface;
using Domain.Calculator;
using Domain.Services;
using System.Threading.Tasks;
using Pokemon.Infrastructure.Factories;
using Pokemon.Infrastructure.Interfaces.AI;
using Pokemon.Infrastructure.Services.AI;
using Pokemon.AppServices.Interfaces;
using Pokemon.AppServices.Mappers;

namespace PokemonBattle
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            //https://learn.microsoft.com/sv-se/dotnet/core/extensions/dependency-injection/usage
            //LogService(transientService, "Always different");
            //LogService(scopedService, "Changes only with lifetime");
            //LogService(singletonService, "Always the same");
            // Sample output:
            // Lifetime 1: Call 1 to provider.GetRequiredService<ServiceLifetimeReporter>()
            //     IExampleTransientService: d08a27fa-87d2-4a06-98d7-2773af886125 (Always different)
            //     IExampleScopedService: 402c83c9-b4ed-4be1-b78c-86be1b1d908d (Changes only with lifetime)
            //     IExampleSingletonService: a61f1ff4-0b14-4508-bd41-21d852484a7b (Always the same)
            // ...
            // Lifetime 1: Call 2 to provider.GetRequiredService<ServiceLifetimeReporter>()
            //     IExampleTransientService: b43d68fb-2c7b-4a9b-8f02-fc507c164326 (Always different)
            //     IExampleScopedService: 402c83c9-b4ed-4be1-b78c-86be1b1d908d (Changes only with lifetime)
            //     IExampleSingletonService: a61f1ff4-0b14-4508-bd41-21d852484a7b (Always the same)
            // 
            // Lifetime 2: Call 1 to provider.GetRequiredService<ServiceLifetimeReporter>()
            //     IExampleTransientService: f3856b59-ab3f-4bbd-876f-7bab0013d392 (Always different)
            //     IExampleScopedService: bba80089-1157-4041-936d-e96d81dd9d1c (Changes only with lifetime)
            //     IExampleSingletonService: a61f1ff4-0b14-4508-bd41-21d852484a7b (Always the same)
            // ...
            // Lifetime 2: Call 2 to provider.GetRequiredService<ServiceLifetimeReporter>()
            //     IExampleTransientService: a8015c6a-08cd-4799-9ec3-2f2af9cbbfd2 (Always different)
            //     IExampleScopedService: bba80089-1157-4041-936d-e96d81dd9d1c (Changes only with lifetime)
            //     IExampleSingletonService: a61f1ff4-0b14-4508-bd41-21d852484a7b (Always the same)
            /*
             Från apputdata kan du se följande:
             Transient tjänster är alltid olika. En ny instans skapas med varje hämtning av tjänsten.
             Scoped tjänster ändras endast med ett nytt omfång, men är samma instans inom ett omfång.
             Singleton tjänster är alltid desamma. En ny instans skapas bara en gång.

             */
            //Väldigt viktiga JSON service & storage först så det inte skapar problem senare
            builder.Services.AddSingleton<IJsonStorage, JsonStorage>();
            builder.Services.AddSingleton<ITypeDataLoader, TypeDataLoader>();


            builder.Services.AddTransient<MainPage>();

            builder.Services.AddTransient<MoveViewModel>();
            builder.Services.AddTransient<BattleViewModel>();   
            builder.Services.AddTransient<MoveAssignerPage>();
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddSingleton<ITeamViewModel, TeamViewModel>();

            //Se till att instansiera en HTTP client som kan användas alla services
            builder.Services.AddSingleton(new HttpClient());

            //Services
            builder.Services.AddTransient<IMoveService, MoveService>();
            builder.Services.AddTransient<IImageService, ImageService>();
            builder.Services.AddSingleton<ITeamPokemonService, TeamPokemonService>();
            builder.Services.AddSingleton<IBattleService, BattleService>();
            builder.Services.AddSingleton<IPokemonFetchService, FetchService>();
            builder.Services.AddSingleton<IPokemonFetchRepository, PokemonFetchRepository>();
            builder.Services.AddSingleton<ITypeService, TypeService>();
            builder.Services.AddSingleton<ITypeRepo, TypeRepo>();
            builder.Services.AddSingleton<ITypeModelFactory, TypeModelFactory>();
            builder.Services.AddSingleton<IStatCalculator, GenerationThreeStatCalculator>();
          
            builder.Services.AddSingleton<TypeDataService>();
            builder.Services.AddSingleton<DamageCalculator>();
         
            //Hämtar fil directory till MAUI, som andra delar får veta
            //För redovisning, visa alternativ metod
            builder.Services.AddSingleton<IMauiStorageDirectoryHelper, MauiStorageDirectoryHelperService>();

            //AI logik
            builder.Services.AddSingleton<IAIService, AIService>();
            builder.Services.AddSingleton<IAiTeamService, AiTeamService>();


            //Facader vi gömmer våra services bakom
            builder.Services.AddTransient<UIFacade>();
            builder.Services.AddTransient<MoveFacade>();
            builder.Services.AddTransient<BattleFacade>();


            //Våra fabriker med DI
            builder.Services.AddSingleton<ListPokemonDisplayModelFactory>();
            builder.Services.AddSingleton<PartyPokemonFactory>();
            builder.Services.AddSingleton<BattlePokemonFactory>();

            //Till fabrikerna
            builder.Services.AddSingleton<ITypeMapper, TypeMapper>();
            builder.Services.AddSingleton<IStatMapper, StatMapper>();
            builder.Services.AddSingleton<IGeneralMapper, GeneralMapper>();

            //Ladda datan för alla typer i förväg (om vi har något)
            var app = builder.Build();
            var loader = app.Services.GetRequiredService<ITypeDataLoader>();
            loader.LoadTypesFromJsonFolderAsync().GetAwaiter().GetResult();
            return app;
            //return builder.Build();
        }
    }
}
