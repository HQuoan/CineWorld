using System.ComponentModel;

namespace CineWorld.Services.MovieAPI.Models.Dtos
{
  public class MovieDetailsDto
  {
    public MovieDto Movie { get; set; }
    public CategoryDto? Category { get; set; }

    public CountryDto? Country { get; set; }

    public SeriesDto? Series { get; set; }
    public IEnumerable<Episode>? Episodes { get; set; }

    public List<GenreDto> Genres { get; set; } = new List<GenreDto>();

  }
}
