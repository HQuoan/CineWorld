using CineWorld.Services.MovieAPI.Data;
using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Models.Dtos;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CineWorld.Services.MovieAPI.Repositories
{
    public class EpisodeRepository : Repository<Episode>, IEpisodeRepository
  {
    private readonly AppDbContext _db;

    public EpisodeRepository(AppDbContext db): base(db)
    {
      _db = db;
    }


    public async Task<List<EpisodeInforDto>> GetsAsync(Expression<Func<Episode, bool>>? filter = null, string? includeProperties = null)
    {
      IQueryable<Episode> query = _db.Episodes.AsNoTracking();

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
         .Select(e => new EpisodeInforDto
         {
           EpisodeId = e.EpisodeId,
           EpisodeName = e.Name,
           MovieId = e.Movie.MovieId,
           MovieName = e.Movie.Name,
           ImageUrl = e.Movie.ImageUrl,
           EpisodeCurrent = e.Movie.EpisodeCurrent,
           EpisodeTotal = e.Movie.EpisodeTotal,
         })
         .ToListAsync();
    }
  }
}
