using CineWorld.Services.ViewAPI.Data;
using CineWorld.Services.ViewAPI.Repositories.IRepositories;

namespace CineWorld.Services.ViewAPI.Repositories
{
    public class UnitOfWork : IUnitOfWork
  {
    private readonly AppDbContext _db;
    public IViewRepository View { get; private set; }


    public UnitOfWork(AppDbContext db)
    {
      _db = db;
      View = new ViewRepository(_db);
    }

    public async Task SaveAsync()
    {
      await _db.SaveChangesAsync();
    }
  }
}
