using CineWorld.Services.AuthAPI.Models.Dto;
using CineWorld.Services.AuthAPI.Services.IService;
using CineWorld.Services.AuthAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CineWorld.Services.AuthAPI.Controllers
{
  /// <summary>
  /// API for user authentication and management.
  /// </summary>
  [Route("api/auth")]
  [ApiController]
  public class AuthAPIController : ControllerBase
  {
    // Constructor omitted for brevity
    private readonly IAuthService _authService;
    protected ResponseDto _response;
    public AuthAPIController(IAuthService authService)
    {
      _authService = authService;
      _response = new();
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="model">The user registration details.</param>
    /// <returns>A response with the details of the registered user.</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
    {
      var userDto = await _authService.Register(model);
      _response.Result = userDto;
      return Ok(_response);
    }

    /// <summary>
    /// Confirms a user's email address.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <param name="token">The email confirmation token.</param>
    /// <returns>A success message if the email is confirmed.</returns>
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

    /// <summary>
    /// Logs in a user.
    /// </summary>
    /// <param name="model">The login details.</param>
    /// <returns>A response containing the login result and token if successful.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
    {
      var loginResponse = await _authService.Login(model);
      if (loginResponse.User == null)
      {
        _response.IsSuccess = false;
        _response.Message = "Username or password is incorrect";
        return BadRequest(_response);
      }
      _response.Result = loginResponse;
      _response.Message = loginResponse.Message ?? "";
      return Ok(_response);
    }

    /// <summary>
    /// Assigns a role to a user.
    /// </summary>
    /// <param name="model">The user email and role details.</param>
    /// <returns>A success message if the role is assigned.</returns>
    [HttpPost("AssignRole")]
    [Authorize(Roles = SD.AdminRole)]
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

    /// <summary>
    /// Signs in using Google authentication.
    /// </summary>
    /// <returns>A response containing the authentication result.</returns>
    [HttpPost("signin-google")]
    public async Task<IActionResult> SignInGoogle([FromBody] LoginGoogleRequest request)
    {
      //var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
      //if (!authenticateResult.Succeeded)
      //{
      //  return BadRequest(new { message = "Google authentication failed." });
      //}
      try
      {
        var response = await _authService.SignInWithGoogle(request.Token);
        return Ok(response);
      }
      catch (Exception ex)
      {
        return BadRequest(new { message = ex.Message });
      }
    }

    /// <summary>
    /// Changes the password of the currently authenticated user.
    /// </summary>
    /// <param name="changePasswordDto">The old and new password details.</param>
    /// <returns>A success message if the password is changed.</returns>
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

    /// <summary>
    /// Initiates a password reset for a user.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <returns>A success message if the reset link is sent.</returns>
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

    /// <summary>
    /// Resets the password for a user.
    /// </summary>
    /// <param name="resetPasswordDto">The reset password details.</param>
    /// <returns>A success message if the password is reset.</returns>
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
