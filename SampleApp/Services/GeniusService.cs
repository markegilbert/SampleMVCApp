using System.Web;

namespace SampleApp.Services
{
    public sealed class GeniusService
    {
        private readonly HttpClient _Client;
        private readonly ILogger<GeniusService> _Logger;

        public GeniusService(HttpClient Client, ILogger<GeniusService> Logger)
        {
            if (Client is null) { throw new ArgumentNullException($"The parameter '{nameof(Client)}' was null or otherwise invalid"); }
            if (Logger is null) { throw new ArgumentNullException($"The parameter '{nameof(Logger)}' was null or otherwise invalid"); }

            this._Client = Client;
            this._Logger = Logger;
        }

        public async Task<GeniusSearchResponse> SearchByTrackAndArtistAsync(String? TrackName, String? ArtistName)
        {
            String QueryString;

            QueryString = HttpUtility.HtmlEncode(this.GenerateSearchString(TrackName, ArtistName));
            if (String.IsNullOrEmpty(QueryString)) { return GeniusSearchResponse.GetEmptyObject(); }

            return await _Client.GetFromJsonAsync<GeniusSearchResponse>($"search?q={QueryString}");
        }


        public async Task<String> GetFirstAlbumArtOrDefault(String TrackName, String ArtistName, String DefaultImagePath)
        {
            GeniusSearchResponse SearchResponse;

            SearchResponse = await this.SearchByTrackAndArtistAsync(TrackName, ArtistName);

            if (SearchResponse?.ResponseData?.Hits?.Length > 0)
            {
                if (String.IsNullOrEmpty(SearchResponse.ResponseData.Hits[0].Result.HeaderImageThumbnailURL))
                {
                    #region Logging
                    this._Logger.LogDebug($"\tImage service returned a record, but the HeaderImageThumbnailURL property was null or empty; using the default image");
                    #endregion
                    return DefaultImagePath;
                }

                #region Logging
                this._Logger.LogDebug($"\tImage service returned the image to display");
                #endregion
                return SearchResponse.ResponseData.Hits[0].Result.HeaderImageThumbnailURL;
            }
            else
            {
                #region Logging
                this._Logger.LogDebug($"\tImage service did not return an image; using default image");
                #endregion
                return DefaultImagePath;
            }
        }


        public String GenerateSearchString(String? TrackName, String? ArtistName)
        {
            TrackName = (TrackName ?? "").Trim();
            ArtistName = (ArtistName ?? "").Trim();

            if (String.IsNullOrEmpty(TrackName)) { return ArtistName; }
            if (String.IsNullOrEmpty(ArtistName)) { return TrackName; }
            return String.Format("{0} by {1}", TrackName, ArtistName);
        }

    }
}
