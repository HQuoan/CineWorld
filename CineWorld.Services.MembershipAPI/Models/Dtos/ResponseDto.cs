namespace CineWorld.Services.MembershipAPI.Models.Dtos
{
  public class ResponseDto
  {
    public PaginationDto? Pagination { get; set; }
    public object? Result { get; set; }
    public bool IsSuccess { get; set; } = true;
    public string Message { get; set; } = "";
  }
}
