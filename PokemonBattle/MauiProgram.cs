using Microsoft.Extensions.Logging;
using Pokemon.Infrastructure.Interfaces;
using Pokemon.Infrastructure.Repositories;
using Pokemon.Infrastructure.State;
using Pokemon.Services.Interfaces;
using Pokemon.Services.Services;
using PokemonBattle.Interfaces;
using PokemonBattle.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Pokemon.Infrastructure.Services;
using PokemonBattle.Services;

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

            builder.Services.AddSingleton<ITeamViewModel, TeamViewModel>();
            builder.Services.AddTransient<IPokemonListViewModel, PokemonListViewModel>();

            builder.Services.AddTransient<IImageService, ImageService>();
            builder.Services.AddSingleton<ITeamPokemonService, TeamPokemonService>();
            builder.Services.AddSingleton(new HttpClient());


            builder.Services.AddSingleton<IApplicationState, ApplicationState>();

            builder.Services.AddSingleton<IPokemonFetchService, PokemonFetchService>();
            builder.Services.AddSingleton<IPokemonFetchRepository, PokemonFetchRepository>();

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<MainViewModel>();
            return builder.Build();
        }
    }
}
