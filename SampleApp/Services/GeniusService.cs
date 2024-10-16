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


        public String GenerateUniqueName(String TrackName, String ArtistName)
        {
            TrackName = (TrackName ?? "").Trim();
            ArtistName = (ArtistName ?? "").Trim();

            if (TrackName.Equals(String.Empty) && ArtistName.Equals(String.Empty)) { throw new ArgumentException("At least one parameter needs to have a valid value (something not null, not empty, and not blank)"); }

            if (!TrackName.Equals(String.Empty) && !ArtistName.Equals(String.Empty)) { return $"{TrackName}_{ArtistName}"; }
            if (!TrackName.Equals(String.Empty)) { return TrackName; }
            return ArtistName;
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
