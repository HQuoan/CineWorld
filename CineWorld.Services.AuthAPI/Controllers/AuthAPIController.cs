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

    public AuthAPIController(IAuthService authService)
    {
      _authService = authService;
      _response = new();
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
      try
      {
        var result = await _authService.ConfirmEmail(userId, token);
        return Ok(new { message = "Email confirmed successfully." });
      }
      catch (ApplicationException ex)
      {
        return BadRequest(new { message = ex.Message });
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
