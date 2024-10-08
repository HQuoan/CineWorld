using Microsoft.EntityFrameworkCore;

namespace CineWorld.Services.MembershipAPI.Utilities
{
  public interface IUtil
  {
    List<string> GetUserRoles();
    bool IsInRoles(IEnumerable<string> rolesToCheck);
    bool IsUniqueConstraintViolation(DbUpdateException ex);
  }
}