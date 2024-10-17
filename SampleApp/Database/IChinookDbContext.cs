namespace SampleApp.Database
{
    public interface IChinookDbContext
    {
        Task<ICollection<Track>> FindTrackByNameAndOrArtist(string? TrackName, string? ArtistName);
    }
}
