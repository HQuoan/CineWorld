using CineWorld.Services.MembershipAPI.Data;
using CineWorld.Services.MembershipAPI.Models;
using CineWorld.Services.MembershipAPI.Repositories;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;

namespace CineWorld.Services.MovieAPI.Repositories
{
    public class PackageRepository : Repository<Package>, IPackageRepository
  {
    private readonly AppDbContext _db;

    public PackageRepository(AppDbContext db): base(db)
    {
      _db = db;
    }
  }
}
