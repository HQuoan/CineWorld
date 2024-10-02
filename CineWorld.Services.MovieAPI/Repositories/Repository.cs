using CineWorld.Services.MovieAPI.Data;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CineWorld.Services.MovieAPI.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
  {
    private readonly AppDbContext _db;
    internal DbSet<T> dbSet;

    public Repository(AppDbContext db)
    {
      _db = db;
      this.dbSet = _db.Set<T>();
    }

    public async Task AddAsync(T entity)
    {
      await dbSet.AddAsync(entity);
    }

    public async Task<T> GetAsync(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false)
    {
      IQueryable<T> query = tracked ? dbSet : dbSet.AsNoTracking();

      if (filter != null)
      {
        query = query.Where(filter);
      }

      if (!string.IsNullOrEmpty(includeProperties))
      {
        foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProp);
        }
      }

      return await query.FirstOrDefaultAsync();
    }


    public async Task<IEnumerable<T>> GetAllAsync(QueryParameters<T>? queryParameters = null)
    {
      IQueryable<T> query = dbSet;

      if (queryParameters == null)
      {
        return await query.ToListAsync();
      }

      // Filtering
      if (queryParameters.Filters != null && queryParameters.Filters.Any())
      {
        foreach (var filter in queryParameters.Filters)
        {
          query = query.Where(filter);
        }
      }

      // Including related entities
      if (!string.IsNullOrEmpty(queryParameters.IncludeProperties))
      {
        foreach (var includeProp in queryParameters.IncludeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
          query = query.Include(includeProp.Trim());
        }
      }

      // Sorting
      if (queryParameters.OrderBy != null)
      {
        query = queryParameters.OrderBy(query);
      }

      // Pagination
      if (queryParameters.PageNumber.HasValue && queryParameters.PageSize.HasValue)
      {
        int skip = (queryParameters.PageNumber.Value - 1) * queryParameters.PageSize.Value;
        query = query.Skip(skip).Take(queryParameters.PageSize.Value);
      }

      return await query.ToListAsync();
    }


    public async Task<int> CountAsync(List<Expression<Func<T, bool>>>? filters = null)
    {
      IQueryable<T> query = dbSet;

      if (filters != null && filters.Any())
      {
        foreach (var filter in filters)
        {
          query = query.Where(filter);
        }
      }

      return await query.CountAsync();
    }

    public async Task RemoveAsync(T entity)
    {
      dbSet.Remove(entity);
      await Task.CompletedTask;
    }

    public async Task RemoveRangeAsync(IEnumerable<T> entities)
    {
      dbSet.RemoveRange(entities);
      await Task.CompletedTask;
    }

    public async Task UpdateAsync(T entity)
    {
      dbSet.Update(entity);
      await Task.CompletedTask;
    }
  }
}
