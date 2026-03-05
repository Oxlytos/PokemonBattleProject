using Microsoft.Extensions.Logging;
using Pokemon.Infrastructure.Interfaces;
using Pokemon.Infrastructure.State;
using Pokemon.Services.Interfaces;
using Pokemon.Services.Services;
using PokemonBattle.ViewModels;

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
            builder.Services.AddScoped<IPokemonFetchService, PokemonFetchService>();
            builder.Services.AddScoped<IImageService, ImageService>();

            builder.Services.AddSingleton<IApplicationState, ApplicationState>();

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<MainViewModel>();
            return builder.Build();
        }
    }
}
