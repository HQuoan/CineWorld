using CineWorld.Services.MovieAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.EpisodeAPI.Models
{
  public class Episode
  {
    [Key]
    public int EpisodeId { get; set; }
    [Required]
    public int MovieId { get; set; }
    public Movie Movie { get; set; }
    public string Name { get; set; }
    public int Released { get; set; }
    public bool IsFree { get; set; }
    public bool Status { get; set; }

  }
}
