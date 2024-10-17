using System.Text.Json.Serialization;

namespace SampleApp.Services
{
    public class GeniusSearchResponseHitResult
    {
        [JsonPropertyName("header_image_thumbnail_url")]
        public String? HeaderImageThumbnailURL { get; set; }

        [JsonPropertyName("artist_names")]
        public String? Artist { get; set; }

        [JsonPropertyName("title")]
        public String? Title { get; set; }

        [JsonPropertyName("id")]
        public int TrackID { get; set; }
    }
}
