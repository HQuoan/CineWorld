namespace CineWorld.Services.MovieAPI.Models.Dtos
{
  /// <summary>
  /// Data Transfer Object (DTO) representing a genre with its associated movies.
  /// This class is used to transfer information about a genre and the movies that belong to it.
  /// </summary>
  public class GenreMovieDto
  {
    /// <summary>
    /// Gets or sets the genre associated with the movies.
    /// </summary>
    public GenreDto Genre { get; set; }

    /// <summary>
    /// Gets or sets the list of movies that belong to the specified genre.
    /// </summary>
    public IEnumerable<MovieDto> Movies { get; set; }
  }
}
