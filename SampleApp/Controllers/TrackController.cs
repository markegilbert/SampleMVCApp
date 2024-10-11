using Microsoft.AspNetCore.Mvc;
using SampleApp.Config;
using SampleApp.Database;
using SampleApp.Models;
using SampleApp.Services;

namespace SampleApp.Controllers
{
    public class TrackController : Controller
    {
        private readonly ILogger<HomeController> _Logger;
        private readonly ChinookDbContext _Context;
        private readonly GeniusService _Service;

        public TrackController(ILogger<HomeController> Logger, ChinookDbContext Context, GeniusService Service)
        {
            // TODO: Validate these
            _Logger = Logger;
            _Context = Context;
            _Service = Service;
        }

        public IActionResult Search()
        {
            return View(new TrackSearchModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async Task<IActionResult> Search(TrackSearchModel SearchCriteria)
        {
            GeniusSearchResponse SearchResponse;

            if (!ModelState.IsValid)
            {
                return View(SearchCriteria);
            }

            SearchResponse = await _Service.SearchByTrackAndArtistAsync(SearchCriteria.TrackName, SearchCriteria.ArtistName);

            SearchCriteria.Results = new List<TrackSearchResultModel>();
            SearchCriteria.Results.AddRange((from t in SearchResponse.ResponseData.Hits 
                                             select new TrackSearchResultModel() 
                                             { 
                                                 TrackName = t.Result.Title, 
                                                 TrackId = t.Result.TrackID, 
                                                 ArtistName = t.Result.Artist,
                                                 AlbumArtURL = t.Result.HeaderImageThumbnailURL
                                             }).ToList());

            return View(SearchCriteria);
        }
    }
}
