using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.MovieAPI.Models.Dtos
{
  public class MovieDto
  {
    public int MovieId { get; set; }

    public int CategoryId { get; set; }
   
    public int CountryId { get; set; }
   
    public int? SeriesId { get; set; }
    [Required]
    public string Name { get; set; }
    public string? EnglishName { get; set; }
    public string? Slug { get; set; }
    public int? EpisodeCount { get; set; }
    public int? Duration { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? Trailer { get; set; }
    public string? Year { get; set; }
    public bool IsHot { get; set; }
    public bool Status { get; set; } = true;

  }
}
