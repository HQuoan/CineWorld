using CineWorld.Services.CommentAPI.Models;
using CineWorld.Services.CommentAPI.Models.Dto;
using CineWorld.Services.CommentAPI.Models.Dtos;
using CineWorld.Services.CommentAPI.Services.IService;
using Newtonsoft.Json;

namespace CineWorld.Services.CommentAPI.Services
{
    public class CommentService : ICommentService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CommentService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        public async Task<UserInformation> GetUserInformationAsync(string userId)
        {
            var client = _httpClientFactory.CreateClient("UserServiceClient");
            var response = await client.GetAsync($"https://localhost:7000/api/users/GetInfoById/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var userInformation = JsonConvert.DeserializeObject<UserInformation>(content);

                // Kết hợp thông tin người dùng vào CommentDto
                var userReturn = new UserInformation()
                {
                    Id = userInformation.Id,
                    FullName = userInformation.FullName,
                    Avatar = userInformation.Avatar,
                    Gender = userInformation.Gender,
                    DateOfBirth = userInformation.DateOfBirth,
                };

                return userReturn;
            }
            return null;
        }
    }
}
