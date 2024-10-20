using CineWorld.Services.MembershipAPI.Data;
using CineWorld.Services.MembershipAPI.Models;
using CineWorld.Services.MembershipAPI.Repositories.IRepositories;

namespace CineWorld.Services.MembershipAPI.Repositories
{
    public class MemberShipRepository : Repository<MemberShip>, IMemberShipRepository
  {
    private readonly AppDbContext _db;

    public MemberShipRepository(AppDbContext db): base(db)
    {
      _db = db;
    }
  }
}
