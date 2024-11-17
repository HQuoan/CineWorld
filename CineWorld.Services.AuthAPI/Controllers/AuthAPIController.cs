using CineWorld.Services.AuthAPI.Models.Dto;
using CineWorld.Services.AuthAPI.Services;
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
  }
}
