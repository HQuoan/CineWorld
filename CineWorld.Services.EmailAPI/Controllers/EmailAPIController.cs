using CineWorld.Services.EmailAPI.Models;
using CineWorld.Services.EmailAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CineWorld.Services.EmailAPI.Controllers
{
  [Route("api/email")]
  [ApiController]
  public class EmailAPIController : ControllerBase
  {
    private readonly IEmailService _emailService;

    public EmailAPIController(IEmailService emailService)
    {
      _emailService = emailService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendEmail([FromBody] EmailRequest emailRequest)
    {
      // Các thông tin thanh toán được truyền vào GenMessage
      await _emailService.SendEmailAsync(emailRequest);
      return Ok("Email sent successfully.");
    }
  }
}
