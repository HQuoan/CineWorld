using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.ViewAPI.Models.Dtos
{
  public class AddViewRequestDto
  {
    [Required]
    public int MovieId { get; set; }
    [Required]
    public int EpisodeId { get; set; }
  }
}
