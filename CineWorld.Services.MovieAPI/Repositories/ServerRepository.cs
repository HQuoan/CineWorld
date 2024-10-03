using CineWorld.Services.MovieAPI.Data;
using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;

namespace CineWorld.Services.MovieAPI.Repositories
{
    public class ServerRepository : Repository<Server>, IServerRepository
  {
    private readonly AppDbContext _db;

    public ServerRepository(AppDbContext db): base(db)
    {
      _db = db;
    }
  }
}
