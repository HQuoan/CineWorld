
using CineWorld.Services.MembershipAPI.Models;

namespace CineWorld.Services.MembershipAPI.Services.IService
{
  public interface IEmailService
  {
    Task<string> SendEmail(EmailRequest emailRequest);
  }
}