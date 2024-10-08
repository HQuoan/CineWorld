using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.MembershipAPI.Models.Dtos
{
  public class CouponDto
  {
    public int CouponId { get; set; }
    [Required]
    public string CouponCode { get; set; }
    [Required]
    public decimal DiscountAmount { get; set; }
    public bool IsActive { get; set; } = true;
    [DefaultValue(10)]
    public int UsageLimit { get; set; } 
    public int UsageCount { get; set; } 
    public DateTime CreatedDate { get; set; }
    [DefaultValue("repeating")]
    public string Duration { get; set; } // "once", "repeating", "forever"
    // Chỉ cần thiết khi Duration là "repeating"
    [DefaultValue(3)]
    public int DurationInMonths { get; set; } = 3;
  }
}
