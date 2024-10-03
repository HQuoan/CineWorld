using CineWorld.Services.EpisodeAPI.Models.Dtos;

namespace CineWorld.Services.EpisodeAPI.Services.IService
{
  public interface IMovieService
  {
    Task<MovieDto>? GetMovie(int id); 
  }
}
