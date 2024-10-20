using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.MembershipAPI.Models
{
  public class MemberShip
  {
    [Key]
    public int MemberShipId { get; set; }
    [Required]
    public string UserId { get; set; }
    [EmailAddress]
    public string? UserEmail { get; set; }
    public DateTime FirstSubscriptionDate { get; set; } // Ngày đăng ký lần đầu
    public DateTime RenewalStartDate { get; set; } // Ngày bắt đầu gia hạn
    public DateTime LastUpdatedDate { get; set; } // Ngày cập nhật lần cuối
    [Required]
    public DateTime ExpirationDate { get; set; } // Ngày hết hạn
  } 
}
