using CineWorld.Services.MembershipAPI.Data;
using CineWorld.Services.MembershipAPI.Models;
using CineWorld.Services.MembershipAPI.Repositories;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;

namespace CineWorld.Services.MovieAPI.Repositories
{
    public class CouponRepository: Repository<Coupon>, ICouponRepository
  {
    private readonly AppDbContext _db;

    public CouponRepository(AppDbContext db): base(db)
    {
      _db = db;
    }
  }
}
