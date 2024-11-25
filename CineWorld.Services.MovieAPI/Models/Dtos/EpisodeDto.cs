using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CineWorld.Services.MovieAPI.Models.Dtos
{
  /// <summary>
  /// Represents a data transfer object for an Episode of a Movie.
  /// </summary>
  public class EpisodeDto
  {
    /// <summary>
    /// Gets or sets the episode ID.
    /// </summary>
    public int EpisodeId { get; set; }

    /// <summary>
    /// Gets or sets the associated Movie ID.
    /// </summary>
    public int MovieId { get; set; }

    /// <summary>
    /// Gets or sets the name of the episode.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the episode number. Must be greater than 0.
    /// Default value is 1.
    /// </summary>
    [DefaultValue(1)]
    [Range(1, int.MaxValue, ErrorMessage = "EpisodeNumber must be greater than 0.")]
    public int EpisodeNumber { get; set; }

    /// <summary>
    /// Gets or sets whether the episode is free to watch. Default value is false.
    /// </summary>
    [DefaultValue(false)]
    public bool IsFree { get; set; }

    /// <summary>
    /// Gets or sets the status of the episode. Defaults to true (active).
    /// </summary>
    public bool Status { get; set; } = true;

    /// <summary>
    /// Gets or sets the date and time when the episode was created.
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the episode was last updated.
    /// </summary>
    public DateTime? UpdatedDate { get; set; }
  }
}
