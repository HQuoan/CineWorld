using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CineWorld.Services.MembershipAPI.Models.Dtos
{
  public class ReceiptDto
  {
    public int ReceiptId { get; set; }
    [Required]
    public string UserId { get; set; }
    [Required]
    public int PackageId { get; set; }
    // public Package? Package { get; set; }
    [DefaultValue(null)]
    public string? CouponCode { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal PackagePrice { get; set; }
    public int TermInMonths { get; set; }
    [NotMapped]
    public decimal TotalAmount => PackagePrice - DiscountAmount;
    public DateTime CreatedDate { get; set; }

    public string? Status { get; set; }
    public string? PaymentIntentId { get; set; }
    public string? StripeSessionId { get; set; }
  }
}
