using CineWorld.Services.MembershipAPI.Models;
using CineWorld.Services.MembershipAPI.Services.IService;
using Newtonsoft.Json;

namespace CineWorld.Services.MembershipAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EmailService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> SendEmail(EmailRequest emailRequest)
        {
            var client = _httpClientFactory.CreateClient("Email");

            // Gửi request POST với body là emailRequest
            var response = await client.PostAsJsonAsync("/api/email/send", emailRequest);

            var apiContent = await response.Content.ReadAsStringAsync();

            return apiContent;
        }
    }
}
