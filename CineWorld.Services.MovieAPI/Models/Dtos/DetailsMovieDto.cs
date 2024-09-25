using System.ComponentModel;

namespace CineWorld.Services.MovieAPI.Models.Dtos
{
  public class DetailsMovieDto
  {
    public int MovieId { get; set; }

    public int? CategoryId { get; set; }
    public CategoryDto? Category { get; set; }

    public int? CountryId { get; set; }
    public CountryDto? Country { get; set; }

    [DefaultValue(null)]
    public int? SeriesId { get; set; }
    public SeriesDto? Series { get; set; }
    public string? Name { get; set; }
    public string? EnglishName { get; set; }
    public string? Slug { get; set; }
    public int? EpisodeCount { get; set; }
    public int? Duration { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? Trailer { get; set; }
    public string? Year { get; set; }
    public bool? IsHot { get; set; }
    public bool? Status { get; set; } = true;

  }
}
