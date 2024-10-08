using CineWorld.Services.MembershipAPI.Data;
using CineWorld.Services.MembershipAPI.Repositories.IRepositories;

namespace CineWorld.Services.MembershipAPI.Repositories
{
    public class UnitOfWork : IUnitOfWork
  {
    private readonly AppDbContext _db;
    public ICouponRepository Coupon { get; private set; }
    public IPackageRepository Package { get; private set; }
    public IReceiptRepository Receipt { get; private set; }


    public UnitOfWork(AppDbContext db)
    {
      _db = db;
      Coupon = new CouponRepository(_db);
      Package = new PackageRepository(_db);
      Receipt = new ReceiptRepository(_db);
    }

    public async Task SaveAsync()
    {
      await _db.SaveChangesAsync();
    }
  }
}
