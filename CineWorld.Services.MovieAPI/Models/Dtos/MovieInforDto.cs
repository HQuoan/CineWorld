namespace CineWorld.Services.MovieAPI.Models.Dtos
{
  public class MovieInforDto
  {
    public int MovieId { get; set; }
    public string MovieName { get; set; }
    public string ImageUrl { get; set; }
    public int? EpisodeCurrent { get; set; }
    public int? EpisodeTotal { get; set; }
  }
}
