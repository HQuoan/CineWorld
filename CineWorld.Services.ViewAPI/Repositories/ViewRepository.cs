using CineWorld.Services.ViewAPI.Repositories.IRepositories;
using CineWorld.Services.ViewAPI.Data;
using CineWorld.Services.ViewAPI.Models;

namespace CineWorld.Services.ViewAPI.Repositories
{
    public class ViewRepository : Repository<View>, IViewRepository
  {
    public ViewRepository(AppDbContext db): base(db)
    {
    }
  }
}
