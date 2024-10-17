using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using SampleApp.Cache;
using SampleApp.Config;
using SampleApp.Database;
using SampleApp.Models;
using SampleApp.Services;
using System.Drawing.Printing;

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

        //public IActionResult Search()
        //{
        //    return View(new TrackSearchModel());
        //}
        public async Task<IActionResult> Search(String TrackName, String ArtistName, int Page)
        {
            // If I do anything other than this
            //      return View(new TrackSearchModel());
            // The ModelState evaluates the TrackName and ArtistName properties individually.
            // That confuses me.  Neither are marked as "Required" any longer, and the error
            // messages that appear are not what the model's Validate() method returns.
            //
            // To avoid this, I clear out the ModelState prior to redisplaying the form.  This 
            // allows the initial page load and a request for a different page of results to
            // display without errors.
            ModelState.Clear();

            if (String.IsNullOrEmpty(TrackName) && String.IsNullOrEmpty(ArtistName))
            {
                return View(new TrackSearchModel());
            }

            return await Search(new TrackSearchModel() { TrackName = TrackName, ArtistName = ArtistName, Page = Page });
        }

        // TODO: The object name SearchCriteria is not correct - this model holds both search criteria and search results. Rename this.
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async Task<IActionResult> Search(TrackSearchModel SearchCriteria)
        {
            ICollection<Track>? SearchResults;

            if (!ModelState.IsValid)
            {
                return View(SearchCriteria);
            }

            // Find the search results
            SearchResults = await this._CacheManager.GetFromCache<ICollection<Track>>(this._ImageService.GenerateUniqueName(SearchCriteria.TrackName, SearchCriteria.ArtistName),
                                                                     async () => await this._Context.FindTrackByNameAndOrArtist(SearchCriteria.TrackName, SearchCriteria.ArtistName));
            if (SearchResults is null) { SearchResults = new List<Track>(); }

            // Load up the new model
            // TODO: This logic should really be moved to the TrackSearchModel class
            SearchCriteria.Page = ((SearchCriteria.Page == 0 && SearchResults.Count > 0) ? 1 : SearchCriteria.Page);
            SearchCriteria.NumberOfResults = SearchResults.Count;
            SearchCriteria.Results = new List<TrackSearchResultModel>();
            SearchCriteria.Results.AddRange((from t in SearchResults
                                             select new TrackSearchResultModel()
                                             {
                                                 TrackName = t.Name,
                                                 TrackId = t.TrackId,
                                                 ArtistName = (t.Composer ?? UNKNOWN_ARTIST_NAME)
                                             })
                                            .Skip((SearchCriteria.Page - 1) * SearchCriteria.NumberOfResultsPerPage).Take(SearchCriteria.NumberOfResultsPerPage)
                                            );

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
