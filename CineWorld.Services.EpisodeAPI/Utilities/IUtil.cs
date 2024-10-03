using Microsoft.EntityFrameworkCore;

namespace CineWorld.Services.EpisodeAPI.Utilities
{
  public interface IUtil
  {
    void FilterMoviesByUserRole<T>(T item);
    List<string> GetUserRoles();
    bool IsInRoles(IEnumerable<string> rolesToCheck);
    bool IsUniqueConstraintViolation(DbUpdateException ex);
  }
}