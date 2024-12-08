using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Foodiefeed.viewmodels;
using Foodiefeed.services;

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

            builder.Services.AddSingleton<UserSession>();
            builder.Services.AddScoped<IThemeHandler, ThemeHandler>();
            builder.Services.AddScoped<IFoodiefeedApiService, FoodiefeedApiService>();


            builder.Services.AddSingleton<UserViewModel>();
            builder.Services.AddSingleton<LogInPage>();
            builder.Services.AddSingleton<SignUpView>();


            builder.Services.AddTransient<BoardViewModel>();
            builder.Services.AddSingleton<BoardPage>();





            #if DEBUG
                builder.Logging.AddDebug();
            #endif
            
            return builder.Build();
        }
    }
}
