using CineWorld.Services.MovieAPI.Models.Dtos;
using CineWorld.Services.MovieAPI.Services.IService;
using Newtonsoft.Json;

namespace CineWorld.Services.MovieAPI.Services
{
    public class MembershipService : IMembershipService
  {
    private readonly IHttpClientFactory _httpClientFactory;

    public MembershipService(IHttpClientFactory httpClientFactory)
    {
      _httpClientFactory = httpClientFactory;
    }

    public async Task<MemberShipDto> GetMembership(string userId)
    {
      try
      {
        var client = _httpClientFactory.CreateClient("Membership");
        var response = await client.GetAsync($"/api/memberships/GetByUserId/{userId}");

        // Nếu phản hồi không thành công, trả về null
        if (!response.IsSuccessStatusCode)
        {
          return null;
        }

        var apiContent = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

        // Nếu resp là null hoặc không thành công, trả về null
        if (resp == null || !resp.IsSuccess)
        {
          return null;
        }

        // Deserialize thành MembershipDto nếu có kết quả
        return JsonConvert.DeserializeObject<MemberShipDto>(Convert.ToString(resp.Result));
      }
      catch (Exception ex)
      {
        return null;
      }
    }

  }
}
