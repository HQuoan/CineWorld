using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.MembershipAPI.Models
{
  public class Coupon
  {
    [Key]
    public int CouponId { get; set; }
    [Required]
    public string CouponCode { get; set; }
    [Required]
    public decimal DiscountAmount { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;
    public int UsageLimit { get; set; } = 10; // Số lần tối đa có thể sử dụng
    public int UsageCount { get; set; } = 0; // Số lần đã sử dụng
    public DateTime CreatedDate { get; set; }
    public string Duration { get; set; } = "repeating"; // "once", "repeating", "forever"
    // Chỉ cần thiết khi Duration là "repeating"
    public int DurationInMonths { get; set; } = 3;
  }
}
