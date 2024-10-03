using CineWorld.Services.MovieAPI.Data;
using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;

namespace CineWorld.Services.MovieAPI.Repositories
{
    public class EpisodeRepository : Repository<Episode>, IEpisodeRepository
  {
    private readonly AppDbContext _db;

    public EpisodeRepository(AppDbContext db): base(db)
    {
      _db = db;
    }
  }
}
