namespace SampleApp.Database
{
    public interface IChinookDbContext
    {
        ICollection<Track> FindTrackByNameAndOrArtist(string? TrackName, string? ArtistName);
    }
}
