using CineWorld.Services.ViewAPI.Models.Dtos;
using CineWorld.Services.ViewAPI.Services.IService;
using Newtonsoft.Json;
using System.Text;

namespace CineWorld.Services.AuthAPI.Services
{
    public class MovieService : IMovieService
  {
    private readonly IHttpClientFactory _httpClientFactory;

    public MovieService(IHttpClientFactory httpClientFactory)
    {
      _httpClientFactory = httpClientFactory;
    }

    public async Task<List<EpisodeInforDto>> GetEpsiodesInfor(IdsRequestDto model)
    {
      var client = _httpClientFactory.CreateClient("Movie");

      // Chuyển đổi model thành JSON body
      var jsonContent = JsonConvert.SerializeObject(model);
      var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

      var response = await client.PostAsync("/api/episodes/GetEpsiodesInfor", content);

      if (!response.IsSuccessStatusCode)
      {
        return null;
      }

      var apiContent = await response.Content.ReadAsStringAsync();
      var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

      if (resp == null)
      {
        return null;
      }

      if (resp.IsSuccess)
      {
        return JsonConvert.DeserializeObject<List<EpisodeInforDto>>(Convert.ToString(resp.Result));
      }

      return null;
    }


    public async Task<List<MovieInforDto>> GetMoviesInfor(IdsRequestDto model)
    {
      var client = _httpClientFactory.CreateClient("Movie");

      // Chuyển đổi model thành JSON body
      var jsonContent = JsonConvert.SerializeObject(model);
      var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

      var response = await client.PostAsync("/api/movies/GetMoviesInfor", content);

      if (!response.IsSuccessStatusCode)
      {
        return null;
      }

      var apiContent = await response.Content.ReadAsStringAsync();
      var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

      if (resp == null)
      {
        return null;
      }

      if (resp.IsSuccess)
      {
        return JsonConvert.DeserializeObject<List<MovieInforDto>>(Convert.ToString(resp.Result));
      }

      return null;
    }

  }
}
