
namespace CineWorld.Services.MovieAPI.Utilities
{
  public interface IUtil
  {
    void FilterMoviesByUserRole<T>(T item);
    List<string> GetUserRoles();
    public bool IsInRoles(IEnumerable<string> rolesToCheck);
  }
}