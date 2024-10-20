using CineWorld.Services.AuthAPI.Utilities;

namespace CineWorld.Services.AuthAPI.Models.Dto
{
  public class UserDto
  {
    public string Id { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public string? Avatar { get; set; }
    public string Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Role { get; set; }
  }
}
