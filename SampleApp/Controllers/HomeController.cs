using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SampleApp.Config;
using SampleApp.Database;
using SampleApp.Models;
using System.Diagnostics;

namespace SampleApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _Logger;
        private readonly ChinookDbContext _Context;

        public HomeController(ILogger<HomeController> Logger, IOptions<GeniusAPISettings> Config, ChinookDbContext Context)
        {
            // TODO: Validate these
            _Logger = Logger;
            //_Config = Config;
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


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
