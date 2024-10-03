using CineWorld.Services.EpisodeAPI.Data;
using CineWorld.Services.EpisodeAPI.Repositories.IRepositories;

namespace CineWorld.Services.EpisodeAPI.Repositories
{
    public class UnitOfWork : IUnitOfWork
  {
    private readonly AppDbContext _db;

    public IEpisodeRepository Episode { get; private set; }
    public IServerRepository Server { get; private set; }

    public UnitOfWork(AppDbContext db)
    {
      _db = db;
      Episode = new EpisodeRepository(db);
      Server = new ServerRepository(db);
  
    }

    public async Task SaveAsync()
    {
      await _db.SaveChangesAsync();
    }
  }
}
