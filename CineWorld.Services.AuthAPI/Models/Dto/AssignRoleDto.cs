using CineWorld.Services.AuthAPI.Utilities;

namespace CineWorld.Services.AuthAPI.Models.Dto
{
  public class AssignRoleDto
  {
    public string Email { get; set; }
    public string Role { get; set; }
  }
}
