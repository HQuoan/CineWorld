using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Models.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CineWorld.Services.MovieAPI.Utilities
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


    public void FilterMoviesByUserRole<T>(T item)
    {
      var roles = GetUserRoles();

      if (roles != null && !roles.Contains("ADMIN"))
      {
        // Sử dụng phản chiếu để lấy thuộc tính Movies
        var moviesProperty = typeof(T).GetProperty("Movies");
        if (moviesProperty != null)
        {
          // Lấy giá trị của thuộc tính Movies
          var moviesValue = moviesProperty.GetValue(item);
          if (moviesValue is IEnumerable<Movie> movies)
          {
            // Lọc các phim có status = true
            var filteredMovies = movies.Where(c => c.Status == true).ToList();
            moviesProperty.SetValue(item, filteredMovies);
          }
          else if (moviesValue is IEnumerable<MovieDto> movieDtos)
          {
            // Lọc các phim có status = true cho MovieDto
            var filteredMovieDtos = movieDtos.Where(c => c.Status == true).ToList();
            moviesProperty.SetValue(item, filteredMovieDtos);
          }
        }
      }
    }



  }
}
