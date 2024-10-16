using Microsoft.EntityFrameworkCore;

namespace SampleApp.Database
{
    // Pattern Source: https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/
    public sealed class ChinookDbContext : DbContext, IChinookDbContext
    {
        private String? _ConnectionString;
        public DbSet<Album> Albums { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Track> Tracks { get; set; }


        public ChinookDbContext(String ConnectionString)
        {
            this._ConnectionString = ConnectionString;
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(this._ConnectionString);
        }


        public ICollection<Track> FindTrackByNameAndOrArtist(String? TrackName, String? ArtistName)
        {
            // TODO: Normalize the parameters
            // TODO: Is there a better way to write this?

            if (!String.IsNullOrEmpty(TrackName) && !String.IsNullOrEmpty(ArtistName)) 
            {
                // TODO: Handle that the Composer may be null
                return this.Tracks.Where(t => t.Name.ToLower().Contains(TrackName.ToLower())
                                        && t.Composer.ToLower().Contains(ArtistName.ToLower()))
                                  .ToList();
            }


            if (!String.IsNullOrEmpty(TrackName))
            {
                return this.Tracks.Where(t => t.Name.ToLower().Contains(TrackName.ToLower()))
                                  .ToList();
            }


            if (!String.IsNullOrEmpty(ArtistName))
            {
                // TODO: Handle that the Composer may be null
                return this.Tracks.Where(t => t.Composer.ToLower().Contains(ArtistName.ToLower()))
                                  .ToList();
            }


            // Simply return an empty list if no criteria were specified
            return new List<Track>();
        }
    }
}
