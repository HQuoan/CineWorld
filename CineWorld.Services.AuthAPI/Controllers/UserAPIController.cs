using AutoMapper;
using CineWorld.Services.AuthAPI.Data;
using CineWorld.Services.AuthAPI.Exceptions;
using CineWorld.Services.AuthAPI.Models;
using CineWorld.Services.AuthAPI.Models.Dto;
using CineWorld.Services.AuthAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Mango.Services.AuthAPI.Controllers
{
  /// <summary>
  /// User management APIs for administrative and user-specific operations.
  /// </summary>
  [Route("api/users")]
  [ApiController]
  [Authorize]
  public class UserAPIController : ControllerBase
  {
    protected ResponseDto _response;
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    public UserAPIController(AppDbContext db, UserManager<ApplicationUser> userManager, IMapper mapper)
    {
      _db = db;
      _userManager = userManager;
      _response = new();
      _mapper = mapper;
    }

    /// <summary>
    /// Retrieves a list of all users. Only accessible by administrators.
    /// </summary>
    /// <returns>A list of users with their roles.</returns>
    /// <response code="200">Returns the list of users.</response>
    /// <response code="403">If the user is not authorized.</response>
    [HttpGet]
    [Authorize(Roles = SD.AdminRole)]
    public async Task<IActionResult> Get()
    {
      var users = await _db.ApplicationUsers.ToListAsync();

      foreach (var user in users)
      {
        var roles = await _userManager.GetRolesAsync(user);
        user.Role = string.Join(", ", roles);
      }

      _response.TotalItems = users.Count;
      _response.Result = _mapper.Map<IEnumerable<UserDto>>(users);

      return Ok(_response);
    }

    /// <summary>
    /// Retrieves detailed information about a specific user by ID.
    /// </summary>
    /// <param name="id">The ID of the user to retrieve.</param>
    /// <returns>The user's details.</returns>
    /// <response code="200">Returns the user information.</response>
    /// <response code="403">If the user is not authorized to view the details.</response>
    /// <response code="404">If the user is not found.</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {

      string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

      if (!User.IsInRole(SD.AdminRole) && (userId != null && userId != id))
      {
        throw new UnauthorizedAccessException("You are not allowed to access data that does not belong to you.");
      }


      var user = await _userManager.FindByIdAsync(id);


      if (user == null)
      {
        throw new NotFoundException($"User with ID: {id} not found.");
      }

      var roles = await _userManager.GetRolesAsync(user);
      user.Role = string.Join(", ", roles);

      _response.Result = _mapper.Map<UserDto>(user);

      return Ok(_response);
    }

    /// <summary>
    /// Checks if a user exists by ID.
    /// </summary>
    /// <param name="id">The ID of the user to check.</param>
    /// <returns>A boolean indicating whether the user exists.</returns>
    /// <response code="200">Returns true if the user exists, false otherwise.</response>
    [HttpGet("IsExistUser/{id}")]
    public async Task<bool> IsExistUser(string id)
    {
      var user = await _userManager.FindByIdAsync(id);

      return user != null;
    }

    [HttpGet("GetInfoById/{userId}")]
    public async Task<IActionResult> GetInfoById(string userId)
    {
      try
      {
        if (userId == null)
        {
          return BadRequest("User ID cannot be empty");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
          return NotFound($"User with ID {userId} not found.");
        }

        var userInfo = new UserInformation
        {
          Id = user.Id,
          FullName = user.FullName,
          Avatar = user.Avatar,
          Gender = user.Gender,
          DateOfBirth = user.DateOfBirth
        };


        return Ok(userInfo);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { Success = false, Message = ex.Message });
      }
    }

    [HttpGet("GetInfoByIds/{id}")]
    public async Task<IActionResult> GetInfoByIds(List<string> userIds)
    {
      try
      {
        if (userIds == null || userIds.Count == 0)
        {
          return BadRequest("User IDs cannot be empty");
        }
        var userInformations = new List<UserInformation>();
        foreach (var userId in userIds)
        {
          var user = await _userManager.FindByIdAsync(userId);
          if (user == null)
          {
            continue;
          }
          var userInfo = new UserInformation
          {
            Id = user.Id,
            FullName = user.FullName,
            Avatar = user.Avatar,
            Gender = user.Gender,
            DateOfBirth = user.DateOfBirth
          };
          userInformations.Add(userInfo);

        }
        return Ok(userInformations);

      }
      catch (Exception ex)
      {
        return StatusCode(500, new { Success = false, Message = ex.Message });
      }
    }

    /// <summary>
    /// Retrieves user details by email.
    /// </summary>
    /// <param name="email">The email of the user to retrieve.</param>
    /// <returns>The user's details.</returns>
    /// <response code="200">Returns the user information.</response>
    /// <response code="403">If the user is not authorized to view the details.</response>
    /// <response code="404">If the user is not found.</response>
    [HttpGet("GetByEmail/{email}")]
    public async Task<IActionResult> GetByEmail(string email)
    {
      string userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

      if (!User.IsInRole(SD.AdminRole) && (userEmail != null && userEmail != email))
      {
        throw new UnauthorizedAccessException("You are not allowed to access data that does not belong to you.");
      }

      var user = await _userManager.FindByEmailAsync(email);
      if (user == null)
      {
        throw new NotFoundException($"User with Email: {email} not found.");
      }

      var roles = await _userManager.GetRolesAsync(user);
      user.Role = string.Join(", ", roles);

      _response.Result = _mapper.Map<UserDto>(user);

      return Ok(_response);
    }

    /// <summary>
    /// Updates user information. Accessible by administrators or the user themselves.
    /// </summary>
    /// <param name="userInformation">The updated user information.</param>
    /// <returns>The updated user information.</returns>
    /// <response code="200">Returns the updated user information.</response>
    /// <response code="403">If the user is not authorized to update the information.</response>
    /// <response code="404">If the user is not found.</response>
    [HttpPut("UpdateInformation")]
    public async Task<IActionResult> UpdateInformation(UserInformation userInformation)
    {
      string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
      if (!User.IsInRole(SD.AdminRole) || (userId != null && userId != userInformation.Id))
      {
        throw new UnauthorizedAccessException("You are not allowed to access data that does not belong to you.");
      }

      var user = await _userManager.FindByIdAsync(userInformation.Id);
      if (user == null)
      {
        throw new NotFoundException($"User with ID: {userInformation.Id} not found.");
      }

      user.FullName = userInformation.FullName;
      user.Avatar = userInformation.Avatar;
      user.Gender = userInformation.Gender;
      user.DateOfBirth = userInformation.DateOfBirth;

      var result = await _userManager.UpdateAsync(user);
      if (!result.Succeeded)
      {
        return BadRequest(result.Errors);
      }

      _response.Result = userInformation;
      return Ok(_response);
    }

    /// <summary>
    /// Deletes a user by ID. Only accessible by administrators.
    /// </summary>
    /// <param name="id">The ID of the user to delete.</param>
    /// <returns>No content if the deletion is successful.</returns>
    /// <response code="204">If the user is deleted successfully.</response>
    /// <response code="403">If the user is not authorized.</response>
    /// <response code="404">If the user is not found.</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = SD.AdminRole)]
    public async Task<IActionResult> Delete(string id)
    {
      var user = await _userManager.FindByIdAsync(id);
      if (user == null)
      {
        throw new NotFoundException($"User with ID: {id} not found.");
      }

      var result = await _userManager.DeleteAsync(user);
      if (!result.Succeeded)
      {
        return BadRequest(result.Errors);
      }

      return NoContent();
    }
  }
}