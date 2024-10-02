using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineWorld.Services.MovieAPI.Models
{
  public class MovieGenre
  {
    [Key]
    public int Id {  get; set; }
    public int MovieId { get; set; }
    public Movie Movie { get; set; }
    [NotMapped]
    public IEnumerable<Movie>? Movies { get; set; }

    public int GenreId { get; set; }
    public Genre Genre { get; set; }
    [NotMapped]
    public IEnumerable<Genre>? Genres { get; set; }
  }
}
