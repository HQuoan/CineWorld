using CineWorld.Services.MovieAPI.Data;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;

namespace CineWorld.Services.MovieAPI.Repositories
{
    public class UnitOfWork : IUnitOfWork
  {
    private readonly AppDbContext _db;
    public ICategoryRepository Category { get; private set; }
    public ICountryRepository Country { get; private set; }
    public IGenreRepository Genre { get; private set; }
    public IMovieRepository Movie { get; private set; }
    public ISeriesRepository Series { get; private set; }

    public UnitOfWork(AppDbContext db)
    {
      _db = db;
      Category = new CategoryRepository(_db);
      Country = new CountryRepository(_db);
      Genre = new GenreRepository(_db);
      Movie = new MovieRepository(_db);
      Series = new SeriesRepository(_db);
    }

    public async Task SaveAsync()
    {
      await _db.SaveChangesAsync();
    }
  }
}
