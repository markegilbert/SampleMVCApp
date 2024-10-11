using Microsoft.EntityFrameworkCore;

namespace SampleApp.Database
{
    public sealed class ChinookDbContext : DbContext
    {
        public DbSet<Album> Albums { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Track> Tracks { get; set; }


        public ChinookDbContext(DbContextOptions<ChinookDbContext> options) : base(options) { }


        public ICollection<Artist> GetAllArtists()
        {
            return this.Artists.OrderBy(a => a.Name).ToList();
        }
        public ICollection<Album> GetAllAlbums()
        {
            return this.Albums.Include(a => a.Tracks).ToList();
        }
        public ICollection<Track> GetAllTracks()
        {
            return this.Tracks.ToList();
        }
    }
}
