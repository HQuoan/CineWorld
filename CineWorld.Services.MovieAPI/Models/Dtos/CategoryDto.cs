using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CineWorld.Services.MovieAPI.Models.Dtos
{
  public class CategoryDto
  {
    public int CategoryId { get; set; }
    [Required]
    public string Name { get; set; }
    public string? Slug { get; set; }
    public bool Status { get; set; } = true;
  }
}
