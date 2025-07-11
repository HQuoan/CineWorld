﻿using CineWorld.Services.MembershipAPI.Utilities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineWorld.Services.MembershipAPI.Models
{
  public class Receipt
  {
    [Key]
    public int ReceiptId { get; set; }
    [Required]
    public string UserId { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public int PackageId { get; set; }
    [ForeignKey("PackageId")]
    public Package Package { get; set; }
    public string? CouponCode { get; set; }
    public decimal DiscountAmount { get; set; }
    [Required]
    public decimal PackagePrice { get; set; }
    [Required]
    public int TermInMonths { get; set; }
    [NotMapped]
    public decimal TotalAmount => PackagePrice - DiscountAmount;
    public DateTime CreatedDate { get; set; }

    public string? Status { get; set; }
    public string? PaymentIntentId { get; set; }
    public string? StripeSessionId { get; set; }
    public string PaymentMethod { get; set; } = SD.PaymentWithStripe;
    public string? PaymentSessionUrl { get; set; }
  }
}
