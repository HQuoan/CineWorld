using CineWorld.Services.MembershipAPI.Models.Dtos;
using System.ComponentModel;

namespace CineWorld.Services.MembershipAPI.Models.Dto
{
  public class StripeRequestDto
  {
    public string? StripeSessionUrl { get; set; }
    public string? StripeSessionId { get; set; }
    [DefaultValue("https://drive.google.com/file/d/1BjNcczy3hcsiLWNdzywwM8ay30MLJdyR/view?usp=sharing")]
    public string? ApprovedUrl { get; set; }
    [DefaultValue("https://drive.google.com/file/d/1BjNcczy3hcsiLWNdzywwM8ay30MLJdyR/view?usp=sharing")]
    public string? CancelUrl { get; set; }
    public ReceiptDto Receipt { get; set; }
  }
}
