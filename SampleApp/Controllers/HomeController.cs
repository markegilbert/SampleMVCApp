using Microsoft.AspNetCore.Mvc;
using SampleApp.Config;
using SampleApp.Database;
using SampleApp.Models;
using System.Diagnostics;

namespace SampleApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _Logger;
        private readonly SampleAppConfig _Config;
        private readonly SampleAppContext _Context;

        public HomeController(ILogger<HomeController> Logger, SampleAppConfig Config, SampleAppContext Context)
        {
            // TODO: Validate these
            _Logger = Logger;
            _Config = Config;
            _Context = Context;

            // TODO: Remove these when done testing
            var Artists = _Context.GetAllArtists();
            _Logger.LogInformation($"Total number of artists found: {Artists.Count}");

            var Albums = _Context.GetAllAlbums();
            _Logger.LogInformation($"Total number of albums found: {Albums.Count}");

            var Tracks = _Context.GetAllTracks();
            _Logger.LogInformation($"Total number of tracks found: {Tracks.Count}");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
