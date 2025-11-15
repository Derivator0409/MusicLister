using Feleves_Feladat_FZW0D1.Services;
using Feleves_Feladat_FZW0D1.ViewModels;
using Feleves_Feladat_FZW0D1.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MusicLister.Infrastructure;
using MusicLister.Services;

namespace Feleves_Feladat_FZW0D1
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
            string connectionString = "Server=szinger.duckdns.org;Port=3306;Database=Teszt;Uid=root;Pwd=Deriv@tor0409;";

            builder.Services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });
            
            builder.Services.AddTransient<MainPage>(); 
            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddScoped<ISongService, DbSongServices>();
            builder.Services.AddScoped<IArtistService, DbArtistServices>();
            builder.Services.AddTransient<EditArtistPage>();
            builder.Services.AddTransient<EditArtistViewModel>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
            
          
        }
    }
}
