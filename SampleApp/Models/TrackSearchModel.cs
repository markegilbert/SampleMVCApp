using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SampleApp.Models
{
    public class TrackSearchModel
    {
        [DisplayName("Track")]
        // TODO: Both of these don't have to be specified; only one or the other
        //[Required(ErrorMessage = "Please specify a track")]
        public String? TrackName { get; set; }

        [DisplayName("Artist")]
        // TODO: Both of these don't have to be specified; only one or the other
        //[Required(ErrorMessage = "Please specify an artist")]
        public String? ArtistName { get; set; }

        public List<TrackSearchResultModel>? Results { get; set; }
    }
}
