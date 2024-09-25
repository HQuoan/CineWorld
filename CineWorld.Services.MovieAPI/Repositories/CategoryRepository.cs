using CineWorld.Services.MovieAPI.Data;
using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;

namespace CineWorld.Services.MovieAPI.Repositories
{
    public class CategoryRepository: Repository<Category>, ICategoryRepository
  {
    private readonly AppDbContext _db;

    public CategoryRepository(AppDbContext db): base(db)
    {
      _db = db;
    }
  }
}
