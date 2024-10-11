using System.Text.Json.Serialization;

namespace SampleApp.Services
{
    public class GeniusResponseMeta
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }
    }
}
