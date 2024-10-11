using Microsoft.AspNetCore.Mvc;
using SampleApp.Config;
using SampleApp.Database;
using SampleApp.Models;

namespace SampleApp.Controllers
{
    public class TrackController : Controller
    {
        private readonly ILogger<HomeController> _Logger;
        private readonly ChinookDbContext _Context;

        public TrackController(ILogger<HomeController> Logger, ChinookDbContext Context)
        {
            // TODO: Validate these
            _Logger = Logger;
            _Context = Context;
        }

        public IActionResult Search()
        {
            return View(new TrackSearchModel());
        }

        [HttpPost]
        public IActionResult Search(TrackSearchModel SearchCriteria)
        {
            if (!ModelState.IsValid)
            {
                return View(SearchCriteria);
            }

            SearchCriteria.Results = new List<TrackSearchResultModel>();
            SearchCriteria.Results.Add(new TrackSearchResultModel() { TrackId = 1, TrackName ="Track 1", AlbumName = "Album 1", AlbumArtURL = "https://images.genius.com/543ab2a84a7f3c98590a88aa2c80215b.630x630x1.jpg" });
            SearchCriteria.Results.Add(new TrackSearchResultModel() { TrackId = 2, TrackName = "Track 2", AlbumName = "Album 2", AlbumArtURL = "https://images.genius.com/543ab2a84a7f3c98590a88aa2c80215b.630x630x1.jpg" });
            SearchCriteria.Results.Add(new TrackSearchResultModel() { TrackId = 3, TrackName = "Track 3", AlbumName = "Album 3", AlbumArtURL = "https://images.genius.com/543ab2a84a7f3c98590a88aa2c80215b.630x630x1.jpg" });

            return View(SearchCriteria);
        }
    }
}
