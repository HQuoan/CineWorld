using Microsoft.AspNetCore.JsonPatch;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CineWorld.Services.EpisodeAPI.Repositories.IRepositories
{
  public interface IRepository<T> where T : class
  {
    // Ex: T - Category
    Task<IEnumerable<T>> GetAllAsync(QueryParameters<T>? queryParameters = null);
    Task<int> CountAsync(List<Expression<Func<T, bool>>>? filters = null);
    Task<T> GetAsync(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task RemoveAsync(T entity);
    Task RemoveRangeAsync(IEnumerable<T> entities);
  }
}
