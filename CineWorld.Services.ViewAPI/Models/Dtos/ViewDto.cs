namespace CineWorld.Services.ViewAPI.Models.Dtos
{
  public class ViewDto
  {
    public int ViewId { get; set; }
    public string? IpAddress { get; set; }
    public string? UserId { get; set; }
    public int MovieId { get; set; }
    public int EpisodeId { get; set; }
    public DateTime ViewDate { get; set; }

    public GetEpsiodeWithMovieInformationDto MovieInfor { get; set; }
  }
}
