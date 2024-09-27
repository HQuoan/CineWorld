using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineWorld.Services.MovieAPI.Models
{
  public class Movie
  {
    [Key]
    public int MovieId { get; set; }
    [Required(ErrorMessage = "CategoryId can't null")]
    public int CategoryId { get; set; }
    [ForeignKey(nameof(CategoryId))]
    public Category? Category { get; set; }
    public int CountryId { get; set; }
    [ForeignKey(nameof(CountryId))]
    public Country? Country { get; set; }
    public int? SeriesId { get; set; }
    [ForeignKey(nameof(SeriesId))]
    public Series? Series { get; set; }

    public ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();

    [Required]
    public string Name { get; set; }
    public string? EnglishName { get; set; }
    [Required]
    public string Slug { get; set; }
    public int? EpisodeCurrent { get; set; }
    public int? EpisodeTotal { get; set; }
    public string? Duration { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? Trailer { get; set; }
    public string? Year { get; set; }
    public bool IsHot { get; set; }
    public bool Status { get; set; } = true;

  }
}
