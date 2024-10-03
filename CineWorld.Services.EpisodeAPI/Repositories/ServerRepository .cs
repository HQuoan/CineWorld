using CineWorld.Services.EpisodeAPI.Data;
using CineWorld.Services.EpisodeAPI.Models;
using CineWorld.Services.EpisodeAPI.Repositories.IRepositories;

namespace CineWorld.Services.EpisodeAPI.Repositories
{
    public class ServerRepository: Repository<Server>, IServerRepository
  {
    private readonly AppDbContext _db;

    public ServerRepository(AppDbContext db): base(db)
    {
      _db = db;
    }
  }
}
