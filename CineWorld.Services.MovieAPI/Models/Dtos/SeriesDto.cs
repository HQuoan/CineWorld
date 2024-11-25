using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.MovieAPI.Models.Dtos
{
  /// <summary>
  /// Data Transfer Object (DTO) for a movie series.
  /// This class is used to transfer data about a movie series between the API and the client.
  /// </summary>
  public class SeriesDto
  {
    /// <summary>
    /// Gets or sets the unique identifier for the series.
    /// </summary>
    public int SeriesId { get; set; }

    /// <summary>
    /// Gets or sets the name of the series.
    /// This field is required.
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the slug (a URL-friendly version of the name) for the series.
    /// This field is optional.
    /// </summary>
    public string? Slug { get; set; }

    /// <summary>
    /// Gets or sets the status of the series (active or inactive).
    /// The default value is true (active).
    /// </summary>
    public bool Status { get; set; } = true;
  }
}
