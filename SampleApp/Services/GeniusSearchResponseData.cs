using System.Text.Json.Serialization;

namespace SampleApp.Services
{
    public class GeniusSearchResponseData
    {
        [JsonPropertyName("hits")]
        public GeniusSearchResponseHit[] Hits { get; set; }
    }
}
