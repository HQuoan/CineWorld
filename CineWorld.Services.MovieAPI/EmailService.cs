using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace CineWorld.Services.MovieAPI
{
  public interface IEmailService
  {
    Task SendEmailAsync(string to, string subject, string message);
  }

  public class EmailService : IEmailService
  {
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string message)
    {
      var email = new MimeMessage();
      email.From.Add(MailboxAddress.Parse(_configuration["Smtp:Username"]));
      email.To.Add(MailboxAddress.Parse(to));
      email.Subject = subject;


      var bodyBuilder = new BodyBuilder
      {
        //HtmlBody = message

        HtmlBody = GenerateSubscriptionSuccessEmailBody("Vo Minh Huy", "Goi 1 thang" , DateTime.Now)
      };
      email.Body = bodyBuilder.ToMessageBody();

      using (var smtp = new SmtpClient())
      {
        smtp.Connect(_configuration["Smtp:Server"], int.Parse(_configuration["Smtp:Port"]), MailKit.Security.SecureSocketOptions.StartTls);
        smtp.Authenticate(_configuration["Smtp:Username"], _configuration["Smtp:Password"]);
        await smtp.SendAsync(email);
        smtp.Disconnect(true);
      }
    }

    private string GenerateSubscriptionSuccessEmailBody(string userName, string packageName, DateTime endDate)
    {
      return $@"
    <html>
    <body>
        <h2>Đăng ký thành viên thành công!</h2>
        <p>Xin chào {userName},</p>
        <p>Bạn đã đăng ký thành công gói {packageName}. Gói của bạn sẽ hết hạn vào ngày {endDate:dd/MM/yyyy}.</p>
        <p>Chúc bạn có những trải nghiệm tuyệt vời!</p>
        <br/>
        <p>Trân trọng,<br/>Đội ngũ hỗ trợ</p>
    </body>
    </html>";
    }

  }
}
