using CineWorld.EmailService;
using MailKit.Net.Smtp;
using MimeKit;

namespace CineWorld.EmailService
{
  public class EmailService : IEmailService
  {
    public async Task<ResponseEmailDto> SendEmailAsync(EmailRequest emailRequest)
    {
      try
      {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("CineWorld", EmailConfig.USERNAME));
        email.To.Add(new MailboxAddress("Customer", emailRequest.To));
        email.Subject = emailRequest.Subject;

        var bodyBuilder = new BodyBuilder
        {
          HtmlBody = emailRequest.Message
        };
        email.Body = bodyBuilder.ToMessageBody();

        using (var smtp = new SmtpClient())
        {
          await smtp.ConnectAsync(EmailConfig.SERVER, EmailConfig.PORT, MailKit.Security.SecureSocketOptions.StartTls);
          await smtp.AuthenticateAsync(EmailConfig.USERNAME, EmailConfig.PASSWORD);
          await smtp.SendAsync(email);
          await smtp.DisconnectAsync(true);
        }

        return new ResponseEmailDto()
        {
          Message = "Email sent successfully!"
        };
      }
     
      catch
      {
        return await SendEmailViaMailtrapAsync(emailRequest);
      }
    }


    public async Task<ResponseEmailDto> SendEmailWithFallbackAsync(EmailRequest emailRequest)
    {
      try
      {
        // Gửi email qua phương thức chính
        return await SendEmailAsync(emailRequest);
      }
      catch
      {
        // Nếu phương thức chính thất bại, thử gửi qua Mailtrap
        return await SendEmailViaMailtrapAsync(emailRequest);
      }
    }

    private async Task<ResponseEmailDto> SendEmailViaMailtrapAsync(EmailRequest emailRequest)
    {
      try
      {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("CineWorld (Mailtrap)", "noreply@mailtrap.io"));
        email.To.Add(new MailboxAddress("Customer", emailRequest.To));
        email.Subject = emailRequest.Subject;

        var bodyBuilder = new BodyBuilder
        {
          HtmlBody = emailRequest.Message
        };
        email.Body = bodyBuilder.ToMessageBody();

        using (var smtp = new SmtpClient())
        {
          // Cấu hình SMTP Mailtrap
          await smtp.ConnectAsync("smtp.mailtrap.io", 2525, MailKit.Security.SecureSocketOptions.StartTls);
          await smtp.AuthenticateAsync("219cccf354d19a", "977ec2610216db");
          await smtp.SendAsync(email);
          await smtp.DisconnectAsync(true);
        }

        return new ResponseEmailDto()
        {
          IsSuccess = true,
          Message = "Fallback email sent successfully via Mailtrap!"
        };
      }
      catch (Exception ex)
      {
        return new ResponseEmailDto()
        {
          IsSuccess = false,
          Message = $"Failed to send fallback email via Mailtrap: {ex.Message}"
        };
      }
    }
  }
}