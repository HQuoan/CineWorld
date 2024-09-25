using CineWorld.Services.MovieAPI.Data;
using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace CineWorld.Services.MovieAPI.Repositories
{
  public class MovieRepository : Repository<Movie>, IMovieRepository
  {
    private readonly AppDbContext _db;

    public MovieRepository(AppDbContext db) : base(db)
    {
      _db = db;
    }

    public async override Task<Movie> GetAsync(Expression<Func<Movie, bool>>? filter, string? includeProperties = null, bool tracked = false)
    {
      IQueryable<Movie> query;
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
            query = query.Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre);// Tải thể loại liên quan
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
