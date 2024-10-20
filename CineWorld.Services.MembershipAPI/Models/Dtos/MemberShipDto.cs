using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.MembershipAPI.Models.Dtos
{
  public class MemberShipDto
  {
    public int MemberShipId { get; set; }
    [Required]
    public string UserId { get; set; }
    [EmailAddress]
    public string UserEmail { get; set; }
    public DateTime FirstSubscriptionDate { get; set; } = DateTime.UtcNow;// Ngày đăng ký lần đầu
    public DateTime RenewalStartDate { get; set; } = DateTime.UtcNow; // Ngày bắt đầu gia hạn
    public DateTime LastUpdatedDate { get; set; } = DateTime.UtcNow;// Ngày cập nhật lần cuối
    [Required]
    public DateTime ExpirationDate { get; set; } = DateTime.UtcNow;// Ngày hết hạn
  }
}
