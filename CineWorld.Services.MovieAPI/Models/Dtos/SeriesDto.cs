using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.MovieAPI.Models.Dtos
{
  public class SeriesDto
  {
    public int SeriesId { get; set; }
    public string Name { get; set; }
    public string? Slug { get; set; }
    public bool Status { get; set; } = true;
  }
}
