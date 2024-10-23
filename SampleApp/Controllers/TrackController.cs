using Microsoft.AspNetCore.Mvc;
using SampleApp.Cache;
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


        public async Task<IActionResult> Search(String TrackName, String ArtistName, int Page)
        {
            #region Logging
            this._Logger.LogDebug("GET Search");
            #endregion

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
                #region Logging
                this._Logger.LogDebug("\tNeither Track nor Artist were specified; returning the initial view");
                #endregion
                return View(new TrackSearchModel());
            }

            #region Logging
            this._Logger.LogDebug($"\tReturning a pre-populated view with track '{TrackName}' and artist '{ArtistName}', defaulting to page {Page}");
            #endregion
            return await Search(new TrackSearchModel() { TrackName = TrackName, ArtistName = ArtistName, Page = Page });
        }


        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async Task<IActionResult> Search(TrackSearchModel CriteriaAndResults)
        {
            ICollection<Track>? SearchResults;

            #region Logging
            this._Logger.LogDebug("POST Search");
            #endregion

            if (!ModelState.IsValid) 
            {
                #region Logging
                this._Logger.LogDebug($"\tModelState was not valid.");
                #endregion
                return View(CriteriaAndResults); 
            }
            #region Logging
            this._Logger.LogDebug($"\tModelState was valid.");
            #endregion


            // Find the search results
            SearchResults = await this._CacheManager.GetFromCache<ICollection<Track>>(this._CacheManager.GenerateUniqueName(CriteriaAndResults.TrackName, CriteriaAndResults.ArtistName),
                                                                                      async () => await this._Context.FindTrackByNameAndOrArtist(CriteriaAndResults.TrackName, CriteriaAndResults.ArtistName));
            if (SearchResults is null) 
            {
                #region Logging
                this._Logger.LogDebug("\tSearchResults was null; using an empty list");
                #endregion
                SearchResults = new List<Track>(); 
            }
            #region Logging
            this._Logger.LogDebug($"\tSearchResults has {SearchResults.Count} items");
            #endregion


            // Load up the new model
            CriteriaAndResults.TotalNumberOfResults = SearchResults.Count;
            CriteriaAndResults.Results = new List<TrackSearchResultModel>();
            CriteriaAndResults.Results.AddRange((from t in SearchResults
                                             select new TrackSearchResultModel()
                                             {
                                                 TrackName = t.Name,
                                                 TrackId = t.TrackId,
                                                 ArtistName = (t.Composer ?? UNKNOWN_ARTIST_NAME)
                                             })
                                            .Skip((CriteriaAndResults.Page - 1) * CriteriaAndResults.NumberOfResultsPerPage).Take(CriteriaAndResults.NumberOfResultsPerPage)
                                            );

            #region Logging
            this._Logger.LogDebug($"\tDisplaying page {CriteriaAndResults.Page} of {CriteriaAndResults.NumberOfPages} for Track '{CriteriaAndResults.TrackName}' and Artist '{CriteriaAndResults.ArtistName}'");
            #endregion
            return View(CriteriaAndResults);
        }



        public async Task<IActionResult> AlbumArt(String TrackName, String ArtistName)
        {
            String? ImageURL;

            #region Logging
            this._Logger.LogDebug("GET AlbumArt");

            this._Logger.LogDebug($"\tRequesting image for Track '{TrackName}' and Artist '{ArtistName}'");
            #endregion

            ImageURL = await this._CacheManager.GetFromCache<String>(this._CacheManager.GenerateUniqueName(TrackName, ArtistName),
                                                                     () => this._ImageService.GetFirstAlbumArtOrDefault(TrackName, ArtistName, DEFAULT_ALBUM_ART_PATH));

            // TODO: Deal with the nullibility here
            return Content(ImageURL);
        }

    }
}
