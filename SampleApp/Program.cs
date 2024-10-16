
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Options;
using NLog.Extensions.Logging;
using SampleApp.Cache;
using SampleApp.Config;
using SampleApp.Database;
using SampleApp.ErrorHandling;
using SampleApp.Services;
using System;

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


            // Load up the GeniusAPISettings so they can be injected as IOptions<GeniusAPISettings> objects.
            builder.Services.AddOptions<GeniusAPISettings>()
                .BindConfiguration(GeniusAPISettings.SettingsName)
                .ValidateDataAnnotations()
                .ValidateOnStart();
            _LogAs.Info("GeniusAPISettings loaded");


            // This registers your API service class as a transient service with DI.  That means
            // each time it's injected you'll get back a new instance of the class.
            builder.Services.AddHttpClient<GeniusService>((serviceProvider, httpClient) =>
            {
                GeniusAPISettings ServiceSettings = serviceProvider.GetRequiredService<IOptions<GeniusAPISettings>>().Value;

                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ServiceSettings.ClientAccessToken}");
                httpClient.BaseAddress = new Uri("https://api.genius.com");
            });


            // Add services to the container.
            builder.Logging.ClearProviders();
            builder.Logging.AddNLog();

            builder.Services.AddControllersWithViews();

            // Configure GlobalExceptionHandler
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            // Inject DbContext objects for each database
            // Previously I could use a line like this:
            //
            //      builder.Services.AddDbContext<ChinookDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("ChinookDB")));
            //
            // But that injects a ChinookDbContext object.  That won't map to any controllers that expect an IChinookDbContext object.  To solve that,
            // I need to inject an IChinookDbContext object.  To get the connection configured properly, I override the OnConfiguring() method in the
            // ChinookDbContext class, and pass in the connection string via the constructor.
            builder.Services.AddTransient<IChinookDbContext, ChinookDbContext>((serviceProvider) => new ChinookDbContext(builder.Configuration.GetConnectionString("ChinookDB")));


            // Set up and inject the pieces needed for caching
            builder.Services.AddMemoryCache();
            builder.Services.AddTransient<CacheManager>();


            // ************************************************************************************************
            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Configure GlobalExceptionHandler
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
