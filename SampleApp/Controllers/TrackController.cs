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
            ICollection<Track> SearchResults;

            // TODO: At least one criteria has to be specified

            //if (!ModelState.IsValid)
            //{
            //    return View(SearchCriteria);
            //}





            SearchResults = this._Context.FindTrackByNameAndOrArtist(SearchCriteria.TrackName, SearchCriteria.ArtistName);
            SearchCriteria.Results = new List<TrackSearchResultModel>();
            SearchCriteria.Results.AddRange((from t in SearchResults
                                             select new TrackSearchResultModel()
                                             {
                                                 TrackName = t.Name,
                                                 TrackId = t.TrackId,
                                                 ArtistName = t.Composer
                                             }));

            return View(SearchCriteria);
        }

        
        public async Task<IActionResult> AlbumArt(String TrackName, String ArtistName)
        {
            GeniusSearchResponse SearchResponse;

            SearchResponse = await _Service.SearchByTrackAndArtistAsync(TrackName, ArtistName);
            if (SearchResponse?.ResponseData?.Hits?.Length > 0)
            {
                //return Content($"<img src=\"{SearchResponse.ResponseData.Hits[0].Result.HeaderImageThumbnailURL}\">", "text/html; charset=UTF-8");
                return Content(SearchResponse.ResponseData.Hits[0].Result.HeaderImageThumbnailURL);
            }
            else
            {
                // TODO: Return the view with a default image
                return new OkResult();
            }
        }
    }
}
