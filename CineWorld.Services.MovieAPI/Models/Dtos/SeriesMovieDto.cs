namespace CineWorld.Services.MovieAPI.Models.Dtos
{
  /// <summary>
  /// Data Transfer Object (DTO) representing a series and its associated movies.
  /// This class provides a link between a series and the movies that belong to it.
  /// </summary>
  public class SeriesMovieDto
  {
    /// <summary>
    /// Gets or sets the series details.
    /// </summary>
    public SeriesDto Series { get; set; }

    /// <summary>
    /// Gets or sets the list of movies that belong to the series.
    /// </summary>
    public IEnumerable<MovieDto> Movies { get; set; }
  }
}
