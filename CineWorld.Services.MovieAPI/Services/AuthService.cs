using CineWorld.Services.MovieAPI.Models.Dtos;
using CineWorld.Services.MovieAPI.Services.IService;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace CineWorld.Services.MovieAPI.Services
{
  public class AuthService : IAuthService
  {
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthService(IHttpClientFactory httpClientFactory)
    {
      _httpClientFactory = httpClientFactory;
    }

    public async Task<string> UploadImage(IFormFile file, string pictureName)
    {
      var client = _httpClientFactory.CreateClient("User");

      // Đọc nội dung file để gửi đi
      using var memoryStream = new MemoryStream();
      await file.CopyToAsync(memoryStream);
      var fileBytes = memoryStream.ToArray();

      // Tạo request content (chỉ có file được gửi qua form data)
      var formData = new MultipartFormDataContent();
      formData.Add(new ByteArrayContent(fileBytes), "file", file.FileName); // Dùng pictureName làm tên file

      // Tạo URL với query string
      var url = $"api/users/upload?pictureName={pictureName}&folder=movie_images";

      // Gửi POST request
      var response = await client.PostAsync(url, formData);  // Sử dụng URL với query string

      if (response.IsSuccessStatusCode)
      {
        var apiContent = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

        return (string)result.Result; // Trả về URL hoặc chuỗi rỗng nếu không có
      }

      // Xử lý lỗi nếu không thành công
      var errorContent = await response.Content.ReadAsStringAsync();
      return string.Empty; // Trả về chuỗi rỗng nếu thất bại
    }

  }
}
