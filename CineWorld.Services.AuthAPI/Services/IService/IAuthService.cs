using CineWorld.Services.AuthAPI.Models.Dto;

namespace CineWorld.Services.AuthAPI.Services.IService
{
  public interface IAuthService
  {
    Task<UserDto> Register(RegistrationRequestDto registrationRequestDto);
    Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
    Task<bool> AssignRole(string email, string roleName);
  }
}
