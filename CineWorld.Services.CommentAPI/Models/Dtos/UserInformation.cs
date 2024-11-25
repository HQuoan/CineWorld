

namespace CineWorld.Services.CommentAPI.Models.Dto
{
  public class UserInformation
  {
    public string Id { get; set; }
    public string FullName { get; set; }
    public string? Avatar { get; set; }
    public string Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
  }
}
