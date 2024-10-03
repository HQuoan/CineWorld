using CineWorld.Services.EpisodeAPI.Models.Dtos;
using CineWorld.Services.EpisodeAPI.Services.IService;
using Newtonsoft.Json;

namespace CineWorld.Services.EpisodeAPI.Services
{
  public class MovieService : IMovieService
  {
    private readonly IHttpClientFactory _httpClientFactory;

    public MovieService(IHttpClientFactory httpClientFactory)
    {
      _httpClientFactory = httpClientFactory;
    }

    public async Task<MovieDto>? GetMovie(int id)
    {
      var client = _httpClientFactory.CreateClient("Movie");
      var response = await client.GetAsync($"/api/movies/{id}");
      var apiContent = await response.Content.ReadAsStringAsync();
      var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

      if (resp.IsSuccess) {
        return JsonConvert.DeserializeObject<MovieDto>(Convert.ToString(resp.Result));
      }

      return null;
    }
  }
}
