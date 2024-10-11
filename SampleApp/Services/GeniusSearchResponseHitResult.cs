using System.Text.Json.Serialization;

namespace SampleApp.Services
{
    public class GeniusSearchResponseHitResult
    {
        // TODO: Replace this with a link to an image included with this web app if this comes back as null/empty
        [JsonPropertyName("header_image_thumbnail_url")]
        public String? HeaderImageThumbnailURL { get; set; }

        // TODO: Return value should be "Unknown" if this comes back as null/empty
        [JsonPropertyName("artist_names")]
        public String? Artist { get; set; }

        // TODO: Return value should be "Unknown" if this comes back as null/empty
        [JsonPropertyName("title")]
        public String? Title { get; set; }

        [JsonPropertyName("id")]
        public int TrackID { get; set; }
    }
}
