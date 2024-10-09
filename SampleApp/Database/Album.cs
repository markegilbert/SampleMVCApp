using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SampleApp.Database
{
    public class Album
    {
        public int AlbumId { get; set; }

        [MaxLength(160)]
        public String Title { get; set; } = "";

        public Artist? Artist { get; set; }

        public ICollection<Track>? Tracks { get; set; }
    }
}
