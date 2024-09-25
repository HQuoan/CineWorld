using CineWorld.Services.MovieAPI.Data;
using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;

namespace CineWorld.Services.MovieAPI.Repositories
{
  public class GenreRepository : Repository<Genre>, IGenreRepository
  {
    private readonly AppDbContext _db;

    public GenreRepository(AppDbContext db): base(db)
    {
      _db = db;
    }
  }
}
