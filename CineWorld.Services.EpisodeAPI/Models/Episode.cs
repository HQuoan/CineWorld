using CineWorld.Services.EpisodeAPI.Models.Dtos;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineWorld.Services.EpisodeAPI.Models
{
  public class Episode
  {
    [Key]
    public int EpisodeId { get; set; }
    [Required]
    public int MovieId { get; set; }
    [NotMapped]
    public MovieDto? Movie { get; set; }
    public string Name { get; set; }
    [Required]
    public int EpisodeNumber { get; set; }
    public bool IsFree { get; set; } = false;
    public bool Status { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public IEnumerable<Server> Servers { get; set; }

  }
}
