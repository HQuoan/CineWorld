﻿using CineWorld.EmailService;
using CineWorld.Services.AuthAPI.Data;
using CineWorld.Services.AuthAPI.Models;
using CineWorld.Services.AuthAPI.Models.Dto;
using CineWorld.Services.AuthAPI.Services.IService;
using CineWorld.Services.AuthAPI.Utilities;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Authentication;

namespace CineWorld.Services.AuthAPI.Services
{
    public class AuthService : IAuthService
  {
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IEmailService _emailService;
    //private readonly GoogleSettings _googleSettings;
    private readonly ApiSettings _apiSettings;
    public AuthService(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator,  IEmailService emailService, IOptions<ApiSettings> apiSettings)
    {
      _db = db;
      _userManager = userManager;
      _roleManager = roleManager;
      _jwtTokenGenerator = jwtTokenGenerator;
      _emailService = emailService;
      _apiSettings = apiSettings.Value;
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
      //MemberShipDto membership = new MemberShipDto();
      //try
      //{
      //  membership = await _membershipService.GetMembership(user.Id);
      //}
      //catch (Exception)
      //{
      //  loginResponseDto.Message = "Unable to retrieve membership expiration details from the Membership API. Please try again later.";
      //}

      //var membershipExpiration = DateTime.UtcNow.AddDays(-1);
      //if (membership != null)
      //{
      //  membershipExpiration = membership.ExpirationDate;
      //}

      // if user was found, Generate JWT Token
      var roles = await _userManager.GetRolesAsync(user);
      var token = _jwtTokenGenerator.GenerateToken(user, roles);

      UserDto userDto = new()
      {
        Id = user.Id,
        Email = user.Email,
        FullName = user.FullName,
        Avatar = user.Avatar,
        Gender = user.Gender,
        DateOfBirth = user.DateOfBirth,
        Role = string.Join(", ", roles),
        CreatedDate = user.CreatedDate
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
        CreatedDate = DateTime.UtcNow,
      };

      try
      {
        var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
        if (result.Succeeded)
        {
          // Tạo token và link xác nhận
          var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
          string callBackUrl = $"{_apiSettings.BaseUrl}/api/auth/confirm-email";
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
            throw new BadHttpRequestException($"Registration failed: {emailResponse.Message}");
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
            Role = SD.CustomerRole,
            CreatedDate = user.CreatedDate,
          };

          return userDto;
        }
        else
        {
          var errors = string.Join("; ", result.Errors.Select(e => e.Description));
          throw new BadHttpRequestException($"Registration failed: {errors}");
        }
      }
      catch (Exception ex)
      {
        throw new Exception($"An error occurred during registration. {ex.Message}");
      }
    }

    public async Task<bool> ConfirmEmail(string userId, string token)
    {
      if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
      {
        throw new BadHttpRequestException("Invalid email confirmation request.");
      }

      var user = await _userManager.FindByIdAsync(userId);
      if (user == null)
      {
        throw new BadHttpRequestException("User not found.");
      }

      var result = await _userManager.ConfirmEmailAsync(user, token);
      if (result.Succeeded)
      {
        // Gửi email thông báo xác nhận thành công
        EmailRequest emailRequest = new EmailRequest
        {
          To = user.Email,
          Subject = "Email Confirmed Successfully!",
          Message = "Your email has been confirmed successfully. You can now log in to your account."
        };

        var emailResponse = await _emailService.SendEmailAsync(emailRequest);
        if (!emailResponse.IsSuccess)
        {
          throw new BadHttpRequestException($"Email confirmation succeeded, but failed to send notification email: {emailResponse.Message}");
        }

        return true;
      }
      else
      {
        var errors = string.Join("; ", result.Errors.Select(e => e.Description));
        throw new BadHttpRequestException($"Email confirmation failed: {errors}");
      }
    }
    private async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string token)
    {
      try
      {
        var validPayload = await GoogleJsonWebSignature.ValidateAsync(token);

        // Kiểm tra token hợp lệ bằng ClientId và Audience
        if (validPayload != null&& validPayload.Issuer.ToString() == "https://accounts.google.com" && validPayload.Audience.ToString() == _apiSettings.Google.ClientId)
        {
          return validPayload;
        }
        return null;
      }
      catch (Exception ex)
      {
        return null;
      }
    }

    public async Task<LoginResponseDto> SignInWithGoogle(string token)
    {

      // Xác thực mã token từ Google
      var payload = await VerifyGoogleToken(token);
      if (payload == null)
      {
        throw new UnauthorizedAccessException("Invalid Google token.");
      }

      if (string.IsNullOrEmpty(payload.Email))
      {
        throw new AuthenticationException("Unable to retrieve user email.");
      }

      // Kiểm tra nếu user đã tồn tại trong database, nếu không, tạo mới
      var user = await _userManager.FindByEmailAsync(payload.Email);
      if (user == null)
      {
        user = new ApplicationUser
        {
          Email = payload.Email,
          UserName = payload.Email,
          FullName = payload.Name ?? payload.Email.Split('@')[0],
          NormalizedEmail = payload.Email.ToUpper(),
          Avatar = payload.Picture,
          Gender = "Male",
          DateOfBirth = new DateTime(2000, 1, 1),
          EmailConfirmed = true,
          CreatedDate = DateTime.UtcNow,
        };

        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
          throw new BadHttpRequestException($"Failed to register Google user: {string.Join("; ", result.Errors.Select(e => e.Description))}");
        }

        await _userManager.AddToRoleAsync(user, SD.CustomerRole);

        EmailRequest emailRequest = new EmailRequest()
        {
          To = user.Email,
          Subject = "Register Successfully!",
          Message = $"Register Successfully!"
        };

        var emailResponse = await _emailService.SendEmailAsync(emailRequest);
      }

      // Lấy các vai trò của user
      var roles = await _userManager.GetRolesAsync(user);

      // Tạo token JWT
      LoginResponseDto loginResponseDto = new LoginResponseDto();
      //MemberShipDto membership = new MemberShipDto();
      //try
      //{
      //  membership = await _membershipService.GetMembership(user.Id);
      //}
      //catch (Exception)
      //{
      //  loginResponseDto.Message = "Unable to retrieve membership expiration details from the Membership API. Please try again later.";
      //}

      //var membershipExpiration = DateTime.UtcNow.AddDays(-1);
      //if (membership != null)
      //{
      //  membershipExpiration = membership.ExpirationDate;
      //}
      var tokenRespone = _jwtTokenGenerator.GenerateToken(user, roles);


      UserDto userDto = new()
      {
        Email = user.Email,
        FullName = user.FullName,
        Avatar = user.Avatar,
        Gender = "Male",
        DateOfBirth = new DateTime(2000, 1, 1),
        Role = string.Join(", ", roles),
        CreatedDate = user.CreatedDate,
      };

      loginResponseDto.User = userDto;
      loginResponseDto.Token = tokenRespone;

      return loginResponseDto;
    }

    public async Task<bool> ChangePassword(string userId, ChangePasswordDto changePasswordDto)
    {
      var user = await _userManager.FindByIdAsync(userId);
      if (user == null)
      {
        throw new BadHttpRequestException("User not found.");
      }

      if (user.PasswordHash == null)
      {
        throw new BadHttpRequestException("Password change is not allowed for users who logged in with a Google account.");
      }

      // Kiểm tra mật khẩu cũ
      var passwordValid = await _userManager.CheckPasswordAsync(user, changePasswordDto.CurrentPassword);
      if (!passwordValid)
      {
        throw new BadHttpRequestException("Current password is incorrect.");
      }

      // Kiểm tra mật khẩu mới và xác nhận mật khẩu có khớp không
      if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
      {
        throw new BadHttpRequestException("New password and confirmation password do not match.");
      }

      // Thực hiện thay đổi mật khẩu
      var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
      if (!result.Succeeded)
      {
        var errors = string.Join("; ", result.Errors.Select(e => e.Description));
        throw new BadHttpRequestException($"Password change failed: {errors}");
      }

      return true;
    }
    public async Task<bool> ForgotPassword(string email)
    {
      var user = await _userManager.FindByEmailAsync(email);
      if (user == null)
      {
        throw new BadHttpRequestException("User not found.");
      }

      if (user.PasswordHash == null)
      {
        throw new BadHttpRequestException("Password change is not allowed for users who logged in with a Google account.");
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
        throw new BadHttpRequestException($"Failed to send password reset email: {emailResponse.Message}");
      }

      return true;
    }

    public async Task<bool> ResetPassword(ResetPasswordDto resetPasswordDto)
    {
      var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
      if (user == null)
      {
        throw new BadHttpRequestException("User not found.");
      }

      // Reset mật khẩu
      var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
      if (!result.Succeeded)
      {
        var errors = string.Join("; ", result.Errors.Select(e => e.Description));
        throw new BadHttpRequestException($"Password reset failed: {errors}");
      }

      return true;
    }


  }
}
