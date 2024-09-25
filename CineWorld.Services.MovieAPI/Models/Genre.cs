using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.MovieAPI.Models
{
  public class Genre
  {
    [Key]
    public int GenreId { get; set; }
    [Required]
    public string Name { get; set; }
    public string? Slug { get; set; }
    public bool Status { get; set; } = true;

    public ICollection<MovieGenre>? MovieGenres { get; set; }
  }
}
