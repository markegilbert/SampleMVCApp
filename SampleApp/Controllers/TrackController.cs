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
        private const String DEFAULT_ALBUM_ART_PATH = "../images/AlbumArtDefault.png";
        private const String UNKNOWN_ARTIST_NAME = "Artist Unknown";

        private readonly ILogger<TrackController> _Logger;
        private readonly IChinookDbContext _Context;
        private readonly GeniusService _ImageService;
        private readonly CacheManager _CacheManager;

        public TrackController(ILogger<TrackController> Logger, IChinookDbContext Context, GeniusService ImageService, CacheManager CacheManager)
        {
            if (Logger is null) { throw new ArgumentNullException($"The '{nameof(Logger)}' parameter was null or otherwise invalid"); }
            if (Context is null) { throw new ArgumentNullException($"The '{nameof(Context)}' parameter was null or otherwise invalid"); }
            if (ImageService is null) { throw new ArgumentNullException($"The '{nameof(ImageService)}' parameter was null or otherwise invalid"); }
            if (CacheManager is null) { throw new ArgumentNullException($"The '{nameof(CacheManager)}' parameter was null or otherwise invalid"); }

            this._Logger = Logger;
            this._Context = Context;
            this._ImageService = ImageService;
            this._CacheManager = CacheManager;
        }

        public IActionResult Search()
        {
            return View(new TrackSearchModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public IActionResult Search(TrackSearchModel SearchCriteria)
        {
            ICollection<Track> SearchResults;

            if (!ModelState.IsValid)
            {
                return View(SearchCriteria);
            }

            SearchResults = this._Context.FindTrackByNameAndOrArtist(SearchCriteria.TrackName, SearchCriteria.ArtistName);
            SearchCriteria.Results = new List<TrackSearchResultModel>();
            SearchCriteria.Results.AddRange((from t in SearchResults
                                             select new TrackSearchResultModel()
                                             {
                                                 TrackName = t.Name,
                                                 TrackId = t.TrackId,
                                                 ArtistName = (t.Composer ?? UNKNOWN_ARTIST_NAME)
                                             }));

            return View(SearchCriteria);
        }



        public async Task<IActionResult> AlbumArt(String TrackName, String ArtistName)
        {
            String? ImageURL;

            ImageURL = await this._CacheManager.GetFromCache<String>(this._ImageService.GenerateUniqueName(TrackName, ArtistName), 
                                                                     () => GetFirstAlbumArtOrDefault(TrackName, ArtistName));
            // TODO: Deal with the nullability here
            return Content(ImageURL);
        }

        // TODO: This should be moved out of the controller, and into a wrapper around the GeniusService class (or perhaps incorporated into it)
        private async Task<String> GetFirstAlbumArtOrDefault(String TrackName, String ArtistName)
        {
            GeniusSearchResponse SearchResponse;

            SearchResponse = await _ImageService.SearchByTrackAndArtistAsync(TrackName, ArtistName);
            if (SearchResponse?.ResponseData?.Hits?.Length > 0)
            {
                return (SearchResponse.ResponseData.Hits[0].Result.HeaderImageThumbnailURL ?? DEFAULT_ALBUM_ART_PATH);
            }
            else
            {
                return DEFAULT_ALBUM_ART_PATH;
            }
        }
    }
}
