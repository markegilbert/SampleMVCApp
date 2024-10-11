using System.Text.Json.Serialization;

namespace SampleApp.Services
{
    public class GeniusSearchResponseHit
    {
        [JsonPropertyName("result")]
        public GeniusSearchResponseHitResult Result {  get; set; }
    }
}
