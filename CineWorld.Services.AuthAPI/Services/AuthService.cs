using CineWorld.EmailService;
using CineWorld.Services.AuthAPI.Data;
using CineWorld.Services.AuthAPI.Models;
using CineWorld.Services.AuthAPI.Models.Dto;
using CineWorld.Services.AuthAPI.Services.IService;
using CineWorld.Services.AuthAPI.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CineWorld.Services.AuthAPI.Services
{
  public class AuthService : IAuthService
  {
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IMembershipService _membershipService;
    private readonly IEmailService _emailService;
    public AuthService(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator, IMembershipService membershipService, IEmailService emailService)
    {
      _db = db;
      _userManager = userManager;
      _roleManager = roleManager;
      _jwtTokenGenerator = jwtTokenGenerator;
      _membershipService = membershipService;
      _emailService = emailService;
    }

    public async Task<bool> AssignRole(string email, string roleName)
    {
      var user = _db.ApplicationUsers.FirstOrDefault(c => c.Email.ToLower() == email.ToLower());

      if (user != null)
      {
        if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
        {
          //create role if it does not exit
          _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
        }
        await _userManager.AddToRoleAsync(user, roleName);

        return true;
      }

      return false;
    }

    public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
    {
      var user = _db.ApplicationUsers.FirstOrDefault(c => c.Email.ToLower() == loginRequestDto.Email.ToLower());

      bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

      if (user == null || isValid == false)
      {
        return new LoginResponseDto() { User = null, Token = "" };
      }

      if (!user.EmailConfirmed)
      {
        throw new Exception("Please confirm your email to login.");
      }

      LoginResponseDto loginResponseDto = new LoginResponseDto();
      MemberShipDto membership = new MemberShipDto();
      try
      {
        membership = await _membershipService.GetMembership(user.Id);
      }
      catch (Exception)
      {
        loginResponseDto.Message = "Unable to retrieve membership expiration details from the Membership API. Please try again later.";
      }

      var membershipExpiration = DateTime.UtcNow;
      if (membership != null)
      {
        membershipExpiration = membership.ExpirationDate;
      }

      // if user was found, Generate JWT Token
      var roles = await _userManager.GetRolesAsync(user);
      var token = _jwtTokenGenerator.GenerateToken(user, roles, membershipExpiration);

      UserDto userDto = new()
      {
        Id = user.Id,
        Email = user.Email,
        FullName = user.FullName,
        Gender = user.Gender,
        DateOfBirth = user.DateOfBirth,
        Role = string.Join(", ", roles),
      };


      loginResponseDto.User = userDto;
      loginResponseDto.Token = token;


      return loginResponseDto;
    }

    public async Task<UserDto> Register(RegistrationRequestDto registrationRequestDto)
    {
      ApplicationUser user = new()
      {
        FullName = registrationRequestDto.FullName,
        UserName = registrationRequestDto.Email,
        Email = registrationRequestDto.Email,
        NormalizedEmail = registrationRequestDto.Email.ToUpper(),
        Gender = registrationRequestDto.Gender,
        DateOfBirth = registrationRequestDto.DateOfBirth,
      };

      try
      {
        var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
        if (result.Succeeded)
        {
      

          // Tạo token và link xác nhận
          var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
          string callBackUrl = "https://localhost:7000/api/auth/confirm-email";
          var confirmationLink = $"{callBackUrl}?userId={user.Id}&token={Uri.EscapeDataString(token)}";

          // Gửi email
          EmailRequest emailRequest = new EmailRequest()
          {
            To = user.Email,
            Subject = "Confirm your email",
            Message = $"Please confirm your account by clicking this link: <a href='{confirmationLink}'>Confirm Email</a>"
          };

          var emailResponse = await _emailService.SendEmailAsync(emailRequest);

          if (!emailResponse.IsSuccess)
          {
            await _userManager.DeleteAsync(user);
            throw new ApplicationException($"Registration failed: {emailResponse.Message}");
          }

          // Gán role mặc định là CUSTOMER
          await _userManager.AddToRoleAsync(user, SD.CustomerRole);

          // Nếu gửi email thành công, trả về thông tin user
          UserDto userDto = new()
          {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Gender = user.Gender,
            DateOfBirth = user.DateOfBirth,
            Role = SD.CustomerRole
          };

          return userDto;
        }
        else
        {
          var errors = string.Join("; ", result.Errors.Select(e => e.Description));
          throw new ApplicationException($"Registration failed: {errors}");
        }
      }
      catch (Exception ex)
      {
        throw new Exception($"An error occurred during registration. {ex.Message}");
      }
    }

    public async Task<LoginResponseDto> SignInWithGoogle(AuthenticateResult authenticateResult)
    {

      var claims = authenticateResult.Principal?.Identities.FirstOrDefault()?.Claims;
      var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
      var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

      if (string.IsNullOrEmpty(email))
      {
        throw new ApplicationException("Unable to retrieve user email.");
      }

      // Kiểm tra nếu user đã tồn tại trong database, nếu không, tạo mới
      var user = await _userManager.FindByEmailAsync(email);
      if (user == null)
      {
        user = new ApplicationUser
        {
          Email = email,
          UserName = email,
          FullName = name ?? email.Split('@')[0],
          NormalizedEmail = email.ToUpper(),
          Gender = "Male",
          DateOfBirth = new DateTime(2000, 1, 1),
          EmailConfirmed = true,
        };

        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
          throw new ApplicationException($"Failed to register Google user: {string.Join("; ", result.Errors.Select(e => e.Description))}");
        }

        await _userManager.AddToRoleAsync(user, SD.CustomerRole);
      }

      // Lấy các vai trò của user
      var roles = await _userManager.GetRolesAsync(user);

      // Tạo token JWT
      var membershipExpiration = DateTime.UtcNow; // Lấy membership nếu cần
      var token = _jwtTokenGenerator.GenerateToken(user, roles, membershipExpiration);

      return new LoginResponseDto
      {
        Token = token,
        User = new UserDto
        {
          Id = user.Id,
          Email = user.Email,
          FullName = user.FullName,
          Role = string.Join(", ", roles)
        }
      };
    }

    public async Task<bool> ChangePassword(string userId, ChangePasswordDto changePasswordDto)
    {
      var user = await _userManager.FindByIdAsync(userId);
      if (user == null)
      {
        throw new ApplicationException("User not found.");
      }

      // Kiểm tra mật khẩu cũ
      var passwordValid = await _userManager.CheckPasswordAsync(user, changePasswordDto.CurrentPassword);
      if (!passwordValid)
      {
        throw new ApplicationException("Current password is incorrect.");
      }

      // Kiểm tra mật khẩu mới và xác nhận mật khẩu có khớp không
      if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
      {
        throw new ApplicationException("New password and confirmation password do not match.");
      }

      // Thực hiện thay đổi mật khẩu
      var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
      if (!result.Succeeded)
      {
        var errors = string.Join("; ", result.Errors.Select(e => e.Description));
        throw new ApplicationException($"Password change failed: {errors}");
      }

      return true;
    }
    public async Task<bool> ForgotPassword(string email)
    {
      var user = await _userManager.FindByEmailAsync(email);
      if (user == null)
      {
        throw new ApplicationException("User not found.");
      }

      // Tạo token để đặt lại mật khẩu
      var token = await _userManager.GeneratePasswordResetTokenAsync(user);

      EmailRequest emailRequest = new EmailRequest()
      {
        To = user.Email,
        Subject = "Reset your password",
        Message = $"Your token: {token}"
      };

      var emailResponse = await _emailService.SendEmailAsync(emailRequest);

      if (!emailResponse.IsSuccess)
      {
        throw new ApplicationException($"Failed to send password reset email: {emailResponse.Message}");
      }

      return true;
    }

    public async Task<bool> ResetPassword(ResetPasswordDto resetPasswordDto)
    {
      var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
      if (user == null)
      {
        throw new ApplicationException("User not found.");
      }

      // Reset mật khẩu
      var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
      if (!result.Succeeded)
      {
        var errors = string.Join("; ", result.Errors.Select(e => e.Description));
        throw new ApplicationException($"Password reset failed: {errors}");
      }

      return true;
    }


  }
}
