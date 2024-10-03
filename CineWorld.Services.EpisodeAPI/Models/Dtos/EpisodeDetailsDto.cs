
namespace CineWorld.Services.EpisodeAPI.Models.Dtos
{
  public class EpisodeDetailsDto
  {
    public int EpisodeId { get; set; }
    public int MovieId { get; set; }
    public MovieDto? Movie { get; set; }
    public string Name { get; set; }
    public int EpisodeNumber { get; set; }
    public bool IsFree { get; set; }
    public bool Status { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public IEnumerable<Server>? Servers { get; set; }
  }
}
