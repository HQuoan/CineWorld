namespace CineWorld.Services.MovieAPI.Models.Dtos
{
  /// <summary>
  /// Data Transfer Object (DTO) for a server related to a specific episode.
  /// This class is used to transfer information about a server hosting a movie episode.
  /// </summary>
  public class ServerDto
  {
    /// <summary>
    /// Gets or sets the unique identifier for the server.
    /// </summary>
    public int ServerId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the episode that this server hosts.
    /// </summary>
    public int EpisodeId { get; set; }

    /// <summary>
    /// Gets or sets the name of the server.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the link or URL to access the episode on this server.
    /// </summary>
    public string Link { get; set; }
  }
}
