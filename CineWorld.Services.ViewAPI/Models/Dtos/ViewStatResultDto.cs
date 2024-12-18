namespace CineWorld.Services.ViewAPI.Models.Dtos
{
  public class ViewStatResultDto
  {
    public int Key { get; set; }
    public int ViewCount { get; set; }
    public EpisodeInforDto? EpisodeInfor { get; set; }
    public MovieInforDto? MovieInfor { get; set; }

  }
}
