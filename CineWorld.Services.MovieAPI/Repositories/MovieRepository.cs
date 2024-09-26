using CineWorld.Services.MovieAPI.Data;
using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace CineWorld.Services.MovieAPI.Repositories
{
  public class MovieRepository : Repository<Movie>, IMovieRepository
  {
    private readonly AppDbContext _db;

    public MovieRepository(AppDbContext db) : base(db)
    {
      _db = db;
    }
  }
}
