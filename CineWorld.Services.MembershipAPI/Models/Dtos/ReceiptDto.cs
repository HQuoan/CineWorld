using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CineWorld.Services.MembershipAPI.Models.Dtos
{
  /// <summary>
  /// Represents a receipt for a membership package purchase, including user details, package information, discount, and payment status.
  /// </summary>
  public class ReceiptDto
  {
    /// <summary>
    /// Gets or sets the unique identifier for the receipt. Nullable because it may not be set initially.
    /// </summary>
    public int? ReceiptId { get; set; }

    /// <summary>
    /// Gets or sets the user identifier associated with the receipt.
    /// </summary>
    [Required]
    public string UserId { get; set; }

    /// <summary>
    /// Gets or sets the user's email address associated with the receipt.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the package purchased.
    /// </summary>
    [Required]
    public int PackageId { get; set; }

    /// <summary>
    /// Gets or sets the coupon code applied to the purchase, if any.
    /// Nullable because it may not be provided.
    /// </summary>
    [DefaultValue(null)]
    public string? CouponCode { get; set; }

    /// <summary>
    /// Gets or sets the discount amount applied to the purchase.
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Gets or sets the price of the package before any discount is applied.
    /// </summary>
    public decimal PackagePrice { get; set; }

    /// <summary>
    /// Gets or sets the term duration of the package, in months.
    /// </summary>
    public int TermInMonths { get; set; }

    /// <summary>
    /// Gets the total amount after applying the discount. Calculated as PackagePrice minus DiscountAmount.
    /// </summary>
    [NotMapped]
    public decimal TotalAmount => PackagePrice - DiscountAmount;

    /// <summary>
    /// Gets or sets the date and time when the receipt was created.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the status of the payment (e.g., "Completed", "Pending").
    /// Nullable to indicate it may not be set immediately.
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Gets or sets the payment intent identifier from Stripe, if applicable.
    /// Nullable because it may not be available in some cases.
    /// </summary>
    public string? PaymentIntentId { get; set; }

    /// <summary>
    /// Gets or sets the Stripe session identifier associated with the payment, if applicable.
    /// Nullable because it may not be available in some cases.
    /// </summary>
    public string? StripeSessionId { get; set; }
  }
}
