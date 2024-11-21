using CineWorld.Services.AuthAPI.Models.Dto;
using Microsoft.AspNetCore.Authentication;

namespace CineWorld.Services.AuthAPI.Services.IService
{
  public interface IAuthService
  {
    Task<UserDto> Register(RegistrationRequestDto registrationRequestDto);
    Task<bool> ConfirmEmail(string userId, string token);
    Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
    Task<bool> AssignRole(string email, string roleName);
    Task<LoginResponseDto> SignInWithGoogle(AuthenticateResult authenticateResult);
    Task<bool> ChangePassword(string userId, ChangePasswordDto changePasswordDto);
    Task<bool> ForgotPassword(string email);
    Task<bool> ResetPassword(ResetPasswordDto resetPasswordDto);
  }
}
