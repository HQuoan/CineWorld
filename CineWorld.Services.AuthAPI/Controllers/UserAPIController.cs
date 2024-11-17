using AutoMapper;
using CineWorld.Services.AuthAPI.Data;
using CineWorld.Services.AuthAPI.Exceptions;
using CineWorld.Services.AuthAPI.Models;
using CineWorld.Services.AuthAPI.Models.Dto;
using CineWorld.Services.AuthAPI.Services.IService;
using CineWorld.Services.AuthAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Mango.Services.AuthAPI.Controllers
{
  [Route("api/users")]
  [ApiController]
  //[Authorize]
  public class UserAPIController : ControllerBase
  {
    protected ResponseDto _response;
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    public UserAPIController(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
    {
      _db = db;
      _userManager = userManager;
      _roleManager = roleManager;
      _response = new();
      _mapper = mapper;
    }
    //[Authorize(Roles = SD.AdminRole)]

    /// <summary>
    /// Gets all the cars from the database.
    /// </summary>
    /// <param name="showDeleted">If true, include deleted records.</param>
    /// <param name="pageNumber">The page number to retrieve (zero-based).</param>
    /// <param name="pageSize">The number of records per page.</param>
    /// <returns>A list of cars.</returns>
    /// <response code="200">Returns the list of cars.</response>
    /// <response code="404">No cars found.</response>
    /// <response code="500">Internal server error.</response>
    /// 

    ///
    [HttpGet]
    [Authorize]
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
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="NotFoundException"></exception>
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

    [HttpGet("IsExistUser/{id}")]
    public async Task<bool> IsExistUser(string id)
    {
      var user = await _userManager.FindByIdAsync(id);

      return user != null;
    }

    [HttpGet("GetByEmail/{email}")]
    public async Task<IActionResult> GetByEmail(string email)
    {
      string userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

      if (!User.IsInRole(SD.AdminRole) && (userEmail!= null && userEmail != email))
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

    [HttpPut("UpdateInformation")]
    public async Task<IActionResult> UpdateInformation(UserInformation userInformation)
    {
      string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
      if (!User.IsInRole(SD.AdminRole) ||(userId != null && userId != userInformation.Id)) { 
        throw new UnauthorizedAccessException("You are not allowed to access data that does not belong to you.");
      }

      var user = await _userManager.FindByIdAsync(userInformation.Id);
      if (user == null)
      {
        throw new NotFoundException($"User with ID: {userInformation.Id} not found.");
      }
     
      user.UserName = userInformation.UserName;
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
