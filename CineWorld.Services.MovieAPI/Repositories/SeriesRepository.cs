using CineWorld.Services.MovieAPI.Data;
using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;

namespace CineWorld.Services.MovieAPI.Repositories
{
  public class SeriesRepository : Repository<Series>, ISeriesRepository
  {
    private readonly AppDbContext _db;

    public SeriesRepository(AppDbContext db): base(db)
    {
      _db = db;
    }
  }
}
