using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.ViewAPI.Models
{
  public class View
  {
    [Key]
    public int ViewId { get; set; }
    public string IpAddress { get; set; }
    public string? UserId { get; set; }
    public int MovieId { get; set; }
    public int EpisodeId { get; set; }
    public DateTime ViewDate { get; set; } = DateTime.UtcNow;
  }
}
