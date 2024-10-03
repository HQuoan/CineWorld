namespace CineWorld.Services.EpisodeAPI.Models.Dtos
{
  public class ResponseDto
  {
    public int? TotalItems { get; set; }
    public object? Result { get; set; }
    public bool IsSuccess { get; set; } = true;
    public string Message { get; set; } = "";
  }
}
