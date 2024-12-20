using CineWorld.Services.AuthAPI.Utilities;

namespace CineWorld.Services.AuthAPI.Services
{
  public class ApiSettings
  {
    public string BaseUrl { get; set; }
    public GoogleSettings Google {  get; set; }
  }
}
