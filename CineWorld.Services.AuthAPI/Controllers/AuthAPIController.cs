using CineWorld.Services.AuthAPI.Models;
using CineWorld.Services.AuthAPI.Models.Dto;
using CineWorld.Services.AuthAPI.Services.IService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CineWorld.Services.AuthAPI.Controllers
{
  [Route("api/auth")]
  [ApiController]
  public class AuthAPIController : ControllerBase
  {
    private readonly IAuthService _authService;
    protected ResponseDto _response;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthAPIController(IAuthService authService, UserManager<ApplicationUser> userManager)
    {
      _authService = authService;
      _response = new();
      _userManager = userManager;
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
    {
      var userDto = await _authService.Register(model);
      _response.Result = userDto;

      return Ok(_response);
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
      if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
      {
        return BadRequest(new { message = "Invalid email confirmation request." });
      }

      var user = await _userManager.FindByIdAsync(userId);
      if (user == null)
      {
        return NotFound(new { message = "User not found." });
      }

      var result = await _userManager.ConfirmEmailAsync(user, token);
      if (result.Succeeded)
      {
        return Ok(new { message = "Email confirmed successfully." });
      }
      else
      {
        var errors = string.Join("; ", result.Errors.Select(e => e.Description));
        return BadRequest(new { message = $"Email confirmation failed: {errors}" });
      }
    }



    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
    {

      var loginResponse = await _authService.Login(model);

      if(loginResponse.User == null)
      {
        _response.IsSuccess = false;
        _response.Message = "Username or password is incorrect";

        return BadRequest(_response);
      }

      _response.Result = loginResponse;
      _response.Message = loginResponse.Message ?? "";

      return Ok(_response);
    }

    [HttpPost("AssignRole")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto model)
    {
      var assignRoleSuccessful = await _authService.AssignRole(model.Email, model.Role.ToUpper());

      if (!assignRoleSuccessful)
      {
        _response.IsSuccess = false;
        _response.Message = "Error encountered";

        return BadRequest(_response);
      }

      return Ok(_response);
    }

    [HttpGet("signin-google")]
    [Authorize(Policy = "GoogleAuth")]
    public async Task<IActionResult> SignInGoogle()
    {
      var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
      if (!authenticateResult.Succeeded)
      {
        return BadRequest(new { message = "Google authentication failed." });
      }

      try
      {
        var response = await _authService.SignInWithGoogle(authenticateResult);
        return Ok(response);
      }
      catch (ApplicationException ex)
      {
        return BadRequest(new { message = ex.Message });
      }
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
      var result = await _authService.ChangePassword(userId, changePasswordDto);

      if (result)
      {
        return Ok(new { Message = "Password changed successfully." });
      }

      return BadRequest(new { Message = "Password change failed." });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(string email)
    {
      try
      {
        var result = await _authService.ForgotPassword(email);

        if (result)
        {
          return Ok(new { Message = "Password reset link sent to your email." });
        }
        else
        {
          return BadRequest(new { Message = "Failed to send reset password email." });
        }
      }
      catch (Exception ex)
      {
        return BadRequest(new { Message = ex.Message });
      }
    }


    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
      try
      {
        var result = await _authService.ResetPassword(resetPasswordDto);

        if (result)
        {
          return Ok(new { Message = "Password reset successfully." });
        }
        return BadRequest(new { Message = "Password reset failed." });
      }
      catch (Exception ex)
      {
        return BadRequest(new { Message = ex.Message });
      }
    }
  }
}
