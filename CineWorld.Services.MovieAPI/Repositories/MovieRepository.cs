using CineWorld.Services.MovieAPI.Data;
using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Models.Dtos;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
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

    public async Task<List<MovieInforDto>> GetsAsync(Expression<Func<Movie, bool>>? filter = null, string? includeProperties = null)
    {
      IQueryable<Movie> query = _db.Movies.AsNoTracking();

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

      return await query
         .Select(e => new MovieInforDto
         {
           MovieId = e.MovieId,
           MovieName = e.Name,
           ImageUrl = e.ImageUrl,
           EpisodeCurrent = e.EpisodeCurrent,
           EpisodeTotal = e.EpisodeTotal,
         })
         .ToListAsync();
    }
  }
}
