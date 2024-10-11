using System.Text.Json.Serialization;

namespace SampleApp.Services
{
    public class GeniusSearchResponse
    {
        [JsonPropertyName("meta")]
        public required GeniusResponseMeta Meta { get; set; }

        [JsonPropertyName("response")]
        public required GeniusSearchResponseData ResponseData { get; set; }
    }
}
