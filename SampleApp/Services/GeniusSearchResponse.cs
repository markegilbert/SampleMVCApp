using System.Text.Json.Serialization;

namespace SampleApp.Services
{
    public class GeniusSearchResponse
    {
        [JsonPropertyName("meta")]
        public required GeniusResponseMeta Meta { get; set; }

        [JsonPropertyName("response")]
        public required GeniusSearchResponseData ResponseData { get; set; }


        public static GeniusSearchResponse GetEmptyObject()
        {
            return new GeniusSearchResponse()
            {
                Meta = new GeniusResponseMeta() { Status = 200 },
                ResponseData = new GeniusSearchResponseData() { Hits = new GeniusSearchResponseHit[0] }
            };
        }
    }
}
