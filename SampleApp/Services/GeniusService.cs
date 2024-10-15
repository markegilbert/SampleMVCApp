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

        // TODO: To be unit-tested
        public async Task<GeniusSearchResponse> SearchByTrackAndArtistAsync(String? TrackName, String? ArtistName)
        {
            String QueryString;

            // TODO: Needs to handle when one or both of the parameters are null/empty/blank
            QueryString = HttpUtility.HtmlEncode(String.Format("{0} by {1}", TrackName, ArtistName));

            return await _Client.GetFromJsonAsync<GeniusSearchResponse>($"search?q={QueryString}");
        }


        public String GenerateUniqueName(String TrackName, String ArtistName)
        {
            TrackName = (TrackName ?? String.Empty).Trim();
            ArtistName = (ArtistName ?? String.Empty).Trim();

            if (TrackName.Equals(String.Empty) && ArtistName.Equals(String.Empty)) { throw new ArgumentException("At least one parameter needs to have a valid value (something not null, not empty, and not blank)"); }

            if (!TrackName.Equals(String.Empty) && !ArtistName.Equals(String.Empty)) { return $"{TrackName}_{ArtistName}"; }
            if (!TrackName.Equals(String.Empty)) { return TrackName; }
            return ArtistName;
        }

    }
}
