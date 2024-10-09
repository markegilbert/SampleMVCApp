using System.ComponentModel.DataAnnotations;

namespace SampleApp.Database
{
    public class Track
    {
        public int TrackId { get; set; }

        [MaxLength(200)]
        public String Name { get; set; } = "";

        public int MediaTypeId { get; set; }
        public int GenreId { get; set; }

        [MaxLength(220)]
        public String? Composer { get; set; } = "";

        public int Milliseconds { get; set; }

        public int Bytes { get; set; }

        public decimal UnitPrice { get; set; }

        //public int? AlbumId { get; set; }
        public Album Album { get; set; }
    }
}
