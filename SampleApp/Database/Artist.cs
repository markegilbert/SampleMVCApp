using System.ComponentModel.DataAnnotations;

namespace SampleApp.Database
{
    public class Artist
    {
        public int ArtistId { get; set; }

        [MaxLength(120)]
        public String Name { get; set; } = "";
    }
}
