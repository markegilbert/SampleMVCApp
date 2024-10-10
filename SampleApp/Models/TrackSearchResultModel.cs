using System.ComponentModel.DataAnnotations;

namespace SampleApp.Models
{
    public class TrackSearchResultModel
    {
        public int TrackId { get; set; }
        public String TrackName { get; set; } = "";
        public String AlbumName { get; set; } = "";
        public String? AlbumArtURL { get; set; }
    }
}
