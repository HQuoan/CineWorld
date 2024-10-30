using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CineWorld.Services.MovieAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class EmailController : ControllerBase
  {
    private readonly IEmailService _emailService;

    public EmailController(IEmailService emailService)
    {
      _emailService = emailService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendEmail([FromBody] EmailRequest emailRequest)
    {
      await _emailService.SendEmailAsync(emailRequest.To, emailRequest.Subject, emailRequest.Message);
      return Ok("Email sent successfully.");
    }
  }

  public class EmailRequest
  {
    public string To { get; set; }
    public string Subject { get; set; }
    public string Message { get; set; }
  }
}
