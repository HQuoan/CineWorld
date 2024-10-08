
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CineWorld.Services.MembershipAPI.Utilities
{
  public class Util : IUtil
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public Util(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    public bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
      if (ex.InnerException != null)
      {
        var message = ex.InnerException.Message.ToLower();
        return message.Contains("duplicate key") || message.Contains("unique index");
      }
      return false;
    }


    public List<string> GetUserRoles()
    {
      var user = _httpContextAccessor.HttpContext?.User;
      return user?.Claims
          .Where(c => c.Type == ClaimTypes.Role)
          .Select(c => c.Value)
          .ToList();
    }

    public bool IsInRoles(IEnumerable<string> rolesToCheck)
    {
      var userRoles = GetUserRoles();
      return userRoles != null && rolesToCheck.Any(role => userRoles.Contains(role.ToUpper()));
    }

  }
}
