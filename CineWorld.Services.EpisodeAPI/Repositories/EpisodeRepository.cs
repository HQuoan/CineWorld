using CineWorld.Services.EpisodeAPI.Data;
using CineWorld.Services.EpisodeAPI.Models;
using CineWorld.Services.EpisodeAPI.Repositories.IRepositories;

namespace CineWorld.Services.EpisodeAPI.Repositories
{
    public class EpisodeRepository: Repository<Episode>, IEpisodeRepository
  {
    private readonly AppDbContext _db;

    public EpisodeRepository(AppDbContext db): base(db)
    {
      _db = db;
    }
  }
}
