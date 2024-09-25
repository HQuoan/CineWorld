using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CineWorld.Services.MovieAPI.Models.Dtos
{
  public class SeriesMovieDto
  {
    public SeriesDto Series { get; set; }
    public IEnumerable<MovieDto> Movies { get; set; }

  }
}
