using CineWorld.Services.MovieAPI.Data;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;
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
      IQueryable<T> query;
      if (tracked)
      {
        query = dbSet;
      }
      else
      {
        query = dbSet.AsNoTracking();
      }

      query = query.Where(filter);
      if (!string.IsNullOrEmpty(includeProperties))
      {
        foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
          query = query.Include(includeProp);
        }
      }

      return await query.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
    {
      IQueryable<T> query = dbSet;
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

      return await query.ToListAsync();
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
