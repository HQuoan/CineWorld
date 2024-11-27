using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.MovieAPI.Models.Dtos
{
  /// <summary>
  /// Represents a data transfer object for Country.
  /// </summary>
  public class CountryDto
  {
    /// <summary>
    /// Gets or sets the country ID.
    /// </summary>
    public int CountryId { get; set; }

    /// <summary>
    /// Gets or sets the name of the country. This field is required.
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the slug for the country, used for SEO-friendly URLs.
    /// </summary>
    public string? Slug { get; set; }

    /// <summary>
    /// Gets or sets the status of the country. Defaults to true.
    /// </summary>
    public bool Status { get; set; } = true;
  }
}
