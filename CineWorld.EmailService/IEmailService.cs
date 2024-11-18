namespace CineWorld.EmailService
{
  public interface IEmailService
  {
    Task<ResponseEmailDto> SendEmailAsync(EmailRequest emailRequest);
  }
}