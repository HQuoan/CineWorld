using CineWorld.Services.AuthAPI.Data;
using CineWorld.Services.AuthAPI.Models;
using CineWorld.Services.AuthAPI.Models.Dto;
using CineWorld.Services.AuthAPI.Services.IService;
using Microsoft.AspNetCore.Identity;

namespace CineWorld.Services.AuthAPI.Services
{
  public class AuthService : IAuthService
  {
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    public AuthService(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator)
    {
      _db = db;
      _userManager = userManager;
      _roleManager = roleManager;
      _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<bool> AssignRole(string email, string roleName)
    {
      var user = _db.ApplicationUsers.FirstOrDefault(c => c.Email.ToLower() == email.ToLower());

      if(user != null)
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
      var user = _db.ApplicationUsers.FirstOrDefault(c => c.UserName.ToLower() == loginRequestDto.UserName.ToLower());

      bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

      if (user == null || isValid == false)
      {
        return new LoginResponseDto() { User = null, Token = "" };
      }

      // if user was found, Generate JWT Token
      var roles = await _userManager.GetRolesAsync(user);
      var token = _jwtTokenGenerator.GenerateToken(user, roles);

      UserDto userDto = new()
      {
        Email = user.Email,
        ID = user.Id,
        Name = user.Name,
      };

      LoginResponseDto loginResponseDto = new LoginResponseDto()
      {
        User = userDto,
        Token = token,
      };

      return loginResponseDto;
    }

    public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
    {
      ApplicationUser user = new()
      {
        UserName = registrationRequestDto.Email,
        Email = registrationRequestDto.Email,
        NormalizedEmail = registrationRequestDto.Email.ToUpper(),
        Name = registrationRequestDto.Name,
        Gender = registrationRequestDto.Gender,
        DateOfBirth = registrationRequestDto.DateOfBirth,
      };

      try
      {
        var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
        if (result.Succeeded)
        {
          var userToReturn = _db.ApplicationUsers.First(c => c.UserName == registrationRequestDto.Email);

          UserDto userDto = new()
          {
            Email = userToReturn.Email,
            ID = userToReturn.Id,
            Name = userToReturn.Name,
          };

          return "";
        }
        else
        {
          return result.Errors.FirstOrDefault().Description;
        }
      }catch(Exception ex)
      {

      }

      return "Error Encounterd";
    }
  }
}
