
namespace CineWorld.Services.MovieAPI.Models.Dtos
{
  public class EpisodeDetailsDto
  {
    public EpisodeDto Episode { get; set; }
    public MovieDto? Movie { get; set; }
    public IEnumerable<ServerDto>? Servers { get; set; }
  }
}
