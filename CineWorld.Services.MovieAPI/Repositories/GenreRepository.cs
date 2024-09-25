using CineWorld.Services.MovieAPI.Data;
using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;

namespace CineWorld.Services.MovieAPI.Repositories
{
    public class GenreRepository : Repository<Genre>, IGenreRepository
  {
    private readonly AppDbContext _db;

    public GenreRepository(AppDbContext db): base(db)
    {
      _db = db;
    }

    public async override Task<Genre> GetAsync(Expression<Func<Genre, bool>>? filter, string? includeProperties = null, bool tracked = false)
    {
      IQueryable<Genre> query;
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
          if (includeProp == "MovieGenres")
          {
            query = query.Include(m => m.MovieGenres).ThenInclude(mg => mg.Movie);// Tải phim liên quan
          }
          else
          {
            query = query.Include(includeProp);
          }
        }
      }

      return await query.FirstOrDefaultAsync();
    }
  }
}
