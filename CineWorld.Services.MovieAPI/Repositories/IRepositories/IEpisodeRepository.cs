using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Models.Dtos;
using System.Linq.Expressions;

namespace CineWorld.Services.MovieAPI.Repositories.IRepositories
{
  public interface IEpisodeRepository : IRepository<Episode>
  {
    Task<List<EpisodeInforDto>> GetsAsync(Expression<Func<Episode, bool>>? filter = null, string? includeProperties = null);
  }
}
