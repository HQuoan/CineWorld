using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Models.Dtos;
using System.Linq.Expressions;

namespace CineWorld.Services.MovieAPI.Repositories.IRepositories
{
  public interface IMovieRepository: IRepository<Movie>
  {
    Task<List<MovieInforDto>> GetsAsync(Expression<Func<Movie, bool>>? filter = null, string? includeProperties = null);
  }
}
