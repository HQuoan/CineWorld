using System.ComponentModel;

namespace CineWorld.Services.MovieAPI.Models.Dtos
{
  public class MovieDetailsDto
  {
    public MovieDto Movie { get; set; }
    public CategoryDto? Category { get; set; }

    public CountryDto? Country { get; set; }

    public SeriesDto? Series { get; set; }

  }
}
