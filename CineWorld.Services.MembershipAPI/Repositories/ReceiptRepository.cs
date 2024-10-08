using CineWorld.Services.MembershipAPI.Data;
using CineWorld.Services.MembershipAPI.Models;
using CineWorld.Services.MembershipAPI.Repositories;
using CineWorld.Services.MembershipAPI.Repositories.IRepositories;

namespace CineWorld.Services.MembershipAPI.Repositories
{
    public class ReceiptRepository : Repository<Receipt>, IReceiptRepository
  {
    private readonly AppDbContext _db;

    public ReceiptRepository(AppDbContext db): base(db)
    {
      _db = db;
    }
  }
}
