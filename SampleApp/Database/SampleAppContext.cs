using Microsoft.EntityFrameworkCore;
using SampleApp.Config;
using System.Reflection.Metadata;

namespace SampleApp.Database
{
    public class SampleAppContext : DbContext
    {
        private String _ConnectionString;

        public DbSet<Album> Albums { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Track> Tracks { get; set; }

        public SampleAppContext(SampleAppConfig Config)
        {
            // TODO: Validate this
            this._ConnectionString = Config.ConnectionStrings.Primary;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(this._ConnectionString);



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
