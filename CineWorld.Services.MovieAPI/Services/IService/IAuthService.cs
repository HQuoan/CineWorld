namespace CineWorld.Services.MovieAPI.Services.IService
{
  public interface IAuthService
  {
    Task<string> UploadImage(IFormFile file, string pictureName);
  }
}
