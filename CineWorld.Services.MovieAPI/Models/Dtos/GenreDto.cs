using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.MovieAPI.Models.Dtos
{
  /// <summary>
  /// Data Transfer Object (DTO) representing a movie genre.
  /// This class is used to transfer information about a genre in the system.
  /// </summary>
  public class GenreDto
  {
    /// <summary>
    /// Gets or sets the unique identifier of the genre.
    /// </summary>
    public int GenreId { get; set; }

    /// <summary>
    /// Gets or sets the name of the genre.
    /// This property is required.
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the slug for the genre, used for URL-friendly representations.
    /// This property is optional.
    /// </summary>
    public string? Slug { get; set; }

    /// <summary>
    /// Gets or sets the status of the genre. Indicates whether the genre is active or not.
    /// Default is true, meaning the genre is active.
    /// </summary>
    public bool Status { get; set; } = true;
  }
}
