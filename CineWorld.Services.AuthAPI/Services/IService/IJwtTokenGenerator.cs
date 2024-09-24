using CineWorld.Services.AuthAPI.Models;

namespace CineWorld.Services.AuthAPI.Services.IService
{
  public interface IJwtTokenGenerator
  {
    string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles);
  }
}
