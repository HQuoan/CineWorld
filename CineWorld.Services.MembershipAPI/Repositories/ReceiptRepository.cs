using CineWorld.Services.MembershipAPI.Data;
using CineWorld.Services.MembershipAPI.Models;
using CineWorld.Services.MembershipAPI.Repositories;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;

namespace CineWorld.Services.MovieAPI.Repositories
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
