using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.MovieAPI.Models
{
  public class Country
  {
    [Key]
    public int CountryId { get; set; }
    [Required]
    public string Name { get; set; }
    public string? Slug { get; set; }
    public bool Status { get; set; } = true;
    public IEnumerable<Movie>? Movies { get; set; }
  }
}
