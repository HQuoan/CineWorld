using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.MembershipAPI.Models.Dtos
{
  public class ReceiptDto
  {
    public int ReceiptId { get; set; }
    [Required]
    public int UserId { get; set; }
    [Required]
    public int PackageId { get; set; }
    public Package? Package { get; set; }
    public string? CouponCode { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal PackagePrice { get; set; }
    [NotMapped]
    public decimal TotalAmount => PackagePrice - DiscountAmount;
    public DateTime CreatedDate { get; set; }

    public string? Status { get; set; }
    public string? PaymentIntentId { get; set; }
    public string? StripeSessionId { get; set; }
  }
}
