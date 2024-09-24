using CineWorld.Services.AuthAPI.Utilities;

namespace CineWorld.Services.AuthAPI.Models.Dto
{
  public class RegistrationRequestDto
  {
    public string Email { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public Gender Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Role { get; set; }
  }
}
