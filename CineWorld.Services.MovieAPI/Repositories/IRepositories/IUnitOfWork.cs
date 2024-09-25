namespace CineWorld.Services.MovieAPI.Repositories.IRepositories
{
  public interface IUnitOfWork
  {
    ICategoryRepository Category { get; }
    ICountryRepository Country { get; }
    IGenreRepository Genre { get; }
    IMovieRepository Movie { get; }
    ISeriesRepository Series { get; }
    Task SaveAsync();
  }
}
