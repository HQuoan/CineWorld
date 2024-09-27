using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.MovieAPI.Models
{
  public class Category
  {
    [Key]
    public int CategoryId { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Slug { get; set; }
    public bool Status { get; set; } = true;
    public IEnumerable<Movie>? Movies { get; set; }
  }
}
