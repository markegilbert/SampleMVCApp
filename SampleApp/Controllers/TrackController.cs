using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SampleApp.Cache;
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
        // TODO: Rename this to be more descriptive
        private readonly GeniusService _Service;
        private readonly CacheManager _CacheManager;

        public TrackController(ILogger<HomeController> Logger, ChinookDbContext Context, GeniusService Service, CacheManager CacheManager)
        {
            // TODO: Validate these
            this._Logger = Logger;
            this._Context = Context;
            this._Service = Service;
            this._CacheManager = CacheManager;
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
            String ImageURL;

            // TODO: Create a new method on the GeniusAPI for generating the cache key
            ImageURL = await this._CacheManager.GetFromCache<String>($"{TrackName}_{ArtistName}", () => CallService(TrackName, ArtistName));
            return Content(ImageURL);
        }

        // TODO: Rename this to be more descriptive.
        private async Task<String> CallService(String TrackName, String ArtistName)
        {
            GeniusSearchResponse SearchResponse;

            SearchResponse = await _Service.SearchByTrackAndArtistAsync(TrackName, ArtistName);
            if (SearchResponse?.ResponseData?.Hits?.Length > 0)
            {
                // TODO: Handle the nullability
                return SearchResponse.ResponseData.Hits[0].Result.HeaderImageThumbnailURL;
            }
            else
            {
                // TODO: Return the view with a default image
                return "/Default.png";
            }
        }
    }
}
