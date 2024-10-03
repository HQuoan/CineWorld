using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.EpisodeAPI.Models.Dtos
{
  public class ServerDto
  {
    public int ServerId { get; set; }
    public int EpisodeId { get; set; }
    public string Name { get; set; }
    public string Link { get; set; }
  }
}
