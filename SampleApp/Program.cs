
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NLog.Extensions.Logging;
using SampleApp.Config;
using SampleApp.Database;
using SampleApp.ErrorHandling;

namespace SampleApp
{
    public class Program
    {
        private static NLog.ILogger? _LogAs;


        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            _LogAs = NLog.LogManager.GetCurrentClassLogger();

            _LogAs.Info("");
            _LogAs.Info("************************");
            _LogAs.Info("Program starting");


            // Load up the GeniusAPISettings so they can be injected as IOptions<GeniusAPISettings>.
            builder.Services.AddOptions<GeniusAPISettings>()
                .BindConfiguration(GeniusAPISettings.SettingsName)
                .ValidateDataAnnotations()
                .ValidateOnStart();
            _LogAs.Info("GeniusAPISettings loaded");


            // Add services to the container.
            builder.Logging.ClearProviders();
            builder.Logging.AddNLog();

            builder.Services.AddControllersWithViews();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            // Inject DbContextOptions objects for each database
            builder.Services.AddDbContext<ChinookDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("ChinookDB")));
            


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseExceptionHandler();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }

    }
}
