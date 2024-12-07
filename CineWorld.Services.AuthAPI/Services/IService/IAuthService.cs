using CineWorld.Services.AuthAPI.Models.Dto;

namespace CineWorld.Services.AuthAPI.Services.IService
{
    public interface IAuthService
    {
        Task<bool> AssignRole(string email, string roleName);
        Task<bool> ChangePassword(string userId, ChangePasswordDto changePasswordDto);
        Task<bool> ConfirmEmail(string userId, string token);
        Task<bool> ForgotPassword(string email);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<UserDto> Register(RegistrationRequestDto registrationRequestDto);
        Task<bool> ResetPassword(ResetPasswordDto resetPasswordDto);
        Task<LoginResponseDto> SignInWithGoogle(string token);
    }
}