using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IMemoryCache _Cache;

        public TrackController(ILogger<HomeController> Logger, ChinookDbContext Context, GeniusService Service, IMemoryCache Cache)
        {
            // TODO: Validate these
            this._Logger = Logger;
            this._Context = Context;
            this._Service = Service;
            this._Cache = Cache;
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
            IActionResult? ImageURL = await this._Cache.GetOrCreateAsync<IActionResult>($"{TrackName}_{ArtistName}",
                async _ => await CallService(TrackName, ArtistName));

            return ImageURL;



            //SearchResponse = await _Service.SearchByTrackAndArtistAsync(TrackName, ArtistName);
            //if (SearchResponse?.ResponseData?.Hits?.Length > 0)
            //{
            //    // TODO: Remove this once experiementation is done
            //    //return Content($"<img src=\"{SearchResponse.ResponseData.Hits[0].Result.HeaderImageThumbnailURL}\">", "text/html; charset=UTF-8");

            //    // TODO: Cache the response, using TrackName_ArtistName as the key


            //    return Content(SearchResponse.ResponseData.Hits[0].Result.HeaderImageThumbnailURL);
            //}
            //else
            //{
            //    // TODO: Return the view with a default image
            //    return new OkResult();
            //}
        }

        private async Task<IActionResult> CallService(String TrackName, String ArtistName)
        {
            GeniusSearchResponse SearchResponse;

            SearchResponse = await _Service.SearchByTrackAndArtistAsync(TrackName, ArtistName);
            if (SearchResponse?.ResponseData?.Hits?.Length > 0)
            {
                // TODO: Remove this once experiementation is done
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
