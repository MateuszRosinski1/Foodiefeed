using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Foodiefeed.viewmodels;

namespace Foodiefeed
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");                  
                });

            builder.Services.AddSingleton<BoardViewModel>();
            builder.Services.AddSingleton<BoardPage>();





            #if DEBUG
                builder.Logging.AddDebug();
            #endif
            
            return builder.Build();
        }
    }
}
