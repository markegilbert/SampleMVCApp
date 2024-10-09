
using NLog.Extensions.Logging;
using SampleApp.Config;
using SampleApp.Database;
using SampleApp.ErrorHandling;

namespace SampleApp
{
    public class Program
    {
        private static NLog.ILogger? _LogAs;
        private static SampleAppConfig _Config;


        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            _LogAs = NLog.LogManager.GetCurrentClassLogger();

            _LogAs.Info("");
            _LogAs.Info("************************");
            _LogAs.Info("Program starting");

            _Config = LoadConfig();
            _LogAs.Info("Config loaded");


            // Add services to the container.
            builder.Logging.ClearProviders();
            builder.Logging.AddNLog();

            builder.Services.AddTransient<SampleAppConfig>((x) => _Config);
            builder.Services.AddTransient<SampleAppContext>((x) => LoadDbContext(_Config));
            builder.Services.AddControllersWithViews();

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

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


        private static SampleAppConfig LoadConfig()
        {
            SampleAppConfig NewConfig;

            IConfiguration config = new ConfigurationBuilder()
             .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
             .Build();

            NewConfig = new SampleAppConfig();
            config.Bind(NewConfig);

            return NewConfig;
        }

        private static SampleAppContext LoadDbContext(SampleAppConfig Config)
        {
            return new SampleAppContext(Config);
        }
    }
}
