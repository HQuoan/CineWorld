namespace CineWorld.Services.MovieAPI.Models.Dtos
{
  /// <summary>
  /// Data Transfer Object (DTO) that provides detailed information about an episode.
  /// This class contains the episode details along with related movie and server information.
  /// </summary>
  public class EpisodeDetailsDto
  {
    /// <summary>
    /// Gets or sets the episode information.
    /// </summary>
    public EpisodeDto Episode { get; set; }

    /// <summary>
    /// Gets or sets the movie related to the episode. This may be null if no movie is associated.
    /// </summary>
    public MovieDto? Movie { get; set; }

    /// <summary>
    /// Gets or sets the collection of servers related to the episode. This may be null if no servers are available.
    /// </summary>
    public IEnumerable<ServerDto>? Servers { get; set; }
  }
}
