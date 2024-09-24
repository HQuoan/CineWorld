using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.EpisodeAPI.Models
{
  public class Server
  {
    [Key]
    public int ServerId { get; set; }
    [Required]
    public int EpisodeId { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Link { get; set; }
  }
}
