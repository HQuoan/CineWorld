namespace CineWorld.Services.ViewAPI.Models.Dtos
{
  public class GetEpsiodeWithMovieInformationDto
  {
    public int EpisodeId { get; set; }
    public string EpisodeName { get; set; }
    public int MovieId { get; set; }
    public string MovieName { get; set; }
    public string ImageUrl { get; set; }
    public int? EpisodeCurrent { get; set; }
    public int? EpisodeTotal { get; set; }
  }
}
