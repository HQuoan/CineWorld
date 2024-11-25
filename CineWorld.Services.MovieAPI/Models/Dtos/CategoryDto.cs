using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.MovieAPI.Models.Dtos
{
  /// <summary>
  /// Represents a data transfer object for Category.
  /// </summary>
  public class CategoryDto
  {
    /// <summary>
    /// Gets or sets the category ID.
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Gets or sets the name of the category. This field is required.
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the slug for the category, used for SEO-friendly URLs.
    /// </summary>
    public string? Slug { get; set; }

    /// <summary>
    /// Gets or sets the status of the category. Defaults to true.
    /// </summary>
    public bool Status { get; set; } = true;
  }
}
