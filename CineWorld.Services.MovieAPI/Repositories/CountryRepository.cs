using CineWorld.Services.MovieAPI.Data;
using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;

namespace CineWorld.Services.MovieAPI.Repositories
{
  public class CountryRepository : Repository<Country>, ICountryRepository
  {
    private readonly AppDbContext _db;

    public CountryRepository(AppDbContext db): base(db)
    {
      _db = db;
    }
  }
}
