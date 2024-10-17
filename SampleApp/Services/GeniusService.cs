using System.Web;

namespace SampleApp.Services
{
    public sealed class GeniusService
    {
        private readonly HttpClient _Client;

        public GeniusService(HttpClient client)
        {
            _Client = client;
        }

        public async Task<GeniusSearchResponse> SearchByTrackAndArtistAsync(String? TrackName, String? ArtistName)
        {
            String QueryString;

            QueryString = HttpUtility.HtmlEncode(this.GenerateSearchString(TrackName, ArtistName));
            if (String.IsNullOrEmpty(QueryString)) { return GeniusSearchResponse.GetEmptyObject(); }

            return await _Client.GetFromJsonAsync<GeniusSearchResponse>($"search?q={QueryString}");
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
