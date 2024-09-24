using CineWorld.Services.AuthAPI.Data;
using CineWorld.Services.AuthAPI.Models;
using CineWorld.Services.AuthAPI.Models.Dto;
using CineWorld.Services.AuthAPI.Services.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Mango.Services.AuthAPI.Controllers
{
  [Route("api/user")]
  [ApiController]
  public class UserAPIController : ControllerBase
  {
    protected ResponseDto _response;
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    public UserAPIController(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
      _db = db;
      _userManager = userManager;
      _roleManager = roleManager;
      _response = new();
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
      var users = _db.ApplicationUsers.ToList();
      foreach (var user in users)
      {
        user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();
      }


      _response.Result = users;

      return Ok(_response);
    }
  }
}
