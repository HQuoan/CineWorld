using CineWorld.Services.EmailAPI.Models;

namespace CineWorld.Services.EmailAPI.Services
{
  public interface IEmailService
  {
    Task SendEmailAsync(EmailRequest emailRequest);
  }
}