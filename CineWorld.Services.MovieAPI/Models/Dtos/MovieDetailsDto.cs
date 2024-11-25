namespace CineWorld.Services.MovieAPI.Models.Dtos
{
  /// <summary>
  /// Data Transfer Object (DTO) representing detailed information about a movie.
  /// This class provides a comprehensive view of a movie, including its category, country, series, episodes, and genres.
  /// </summary>
  public class MovieDetailsDto
  {
    /// <summary>
    /// Gets or sets the movie details.
    /// </summary>
    public MovieDto Movie { get; set; }

    /// <summary>
    /// Gets or sets the category of the movie.
    /// </summary>
    public CategoryDto? Category { get; set; }

    /// <summary>
    /// Gets or sets the country where the movie was produced.
    /// </summary>
    public CountryDto? Country { get; set; }

    /// <summary>
    /// Gets or sets the series information, if the movie is part of a series.
    /// </summary>
    public SeriesDto? Series { get; set; }

    /// <summary>
    /// Gets or sets the list of episodes, if the movie is part of a series or multiple episodes.
    /// </summary>
    public IEnumerable<Episode>? Episodes { get; set; }

    /// <summary>
    /// Gets or sets the list of genres associated with the movie.
    /// </summary>
    public List<GenreDto> Genres { get; set; } = new List<GenreDto>();
  }
}
