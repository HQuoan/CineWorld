namespace CineWorld.Services.MovieAPI.Models.Dtos
{
  /// <summary>
  /// Data Transfer Object (DTO) that contains information about a country and its related movies.
  /// This class is used to transfer information about a country and a list of movies associated with that country.
  /// </summary>
  public class CountryMovieDto
  {
    /// <summary>
    /// Gets or sets the country information.
    /// </summary>
    public CountryDto Country { get; set; }

    /// <summary>
    /// Gets or sets the collection of movies related to the country.
    /// </summary>
    public IEnumerable<MovieDto> Movies { get; set; }
  }
}
