using CineWorld.Services.MembershipAPI.Data;
using CineWorld.Services.MembershipAPI.Models;
using CineWorld.Services.MembershipAPI.Repositories;
using CineWorld.Services.MembershipAPI.Repositories.IRepositories;

namespace CineWorld.Services.MembershipAPI.Repositories
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
