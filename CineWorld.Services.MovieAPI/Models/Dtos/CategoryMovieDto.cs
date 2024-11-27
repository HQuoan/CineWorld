namespace CineWorld.Services.MovieAPI.Models.Dtos
{
  public class CategoryMovieDto
  {
    public CategoryDto Category { get; set; }
    public IEnumerable<MovieDto> Movies { get; set; }

  }
}
