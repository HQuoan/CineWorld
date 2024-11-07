using CineWorld.Services.AuthAPI.Models.Dto;
using CineWorld.Services.AuthAPI.Services.IService;
using Newtonsoft.Json;

namespace CineWorld.Services.AuthAPI.Services
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
      var client = _httpClientFactory.CreateClient("Membership");
      var response = await client.GetAsync($"/api/memberships/GetByUserId/{userId}");
      var apiContent = await response.Content.ReadAsStringAsync();
      var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

      if (resp.IsSuccess)
      {
        return JsonConvert.DeserializeObject<MemberShipDto>(Convert.ToString(resp.Result));
      }

      return null;
    }
  }
}
