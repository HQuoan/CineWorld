using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.EpisodeAPI.Models.Dtos
{
  public class CountryDto
  {
    public int CountryId { get; set; }
    [Required]
    public string Name { get; set; }
    public string? Slug { get; set; }
    public bool Status { get; set; } = true;
  }
}
