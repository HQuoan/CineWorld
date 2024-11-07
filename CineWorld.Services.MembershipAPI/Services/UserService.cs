using CineWorld.Services.MembershipAPI.Services.IService;

namespace CineWorld.Services.MembershipAPI.Services
{
    public class UserService : IUserService
  {
    private readonly IHttpClientFactory _httpClientFactory;

    public UserService(IHttpClientFactory httpClientFactory)
    {
      _httpClientFactory = httpClientFactory;
    }

    public async Task<bool> IsExistUser(string userId)
    {
      var client = _httpClientFactory.CreateClient("User");
      var response = await client.GetAsync($"/api/users/IsExistUser/{userId}");

      if (response.IsSuccessStatusCode)
      {
        var apiContent = await response.Content.ReadAsStringAsync();

        if (bool.TryParse(apiContent, out bool result))
        {
          return result;
        }
      }

      return false;
    }

  }
}
