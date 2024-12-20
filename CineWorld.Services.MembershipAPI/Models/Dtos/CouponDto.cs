using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.MembershipAPI.Models.Dtos
{
  /// <summary>
  /// Represents a coupon that can be used to apply discounts to memberships or packages.
  /// </summary>
  public class CouponDto
  {
    /// <summary>
    /// Gets or sets the unique identifier for the coupon.
    /// </summary>
    public int CouponId { get; set; }

    /// <summary>
    /// Gets or sets the coupon code that is used to apply the discount.
    /// </summary>
    /// <example>SUMMER2024</example>
    [Required]
    public string CouponCode { get; set; }

    /// <summary>
    /// Gets or sets the discount amount that the coupon provides.
    /// </summary>
    /// <example>20.5</example>
    [Required]
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the coupon is currently active or not.
    /// </summary>
    /// <default>true</default>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the limit on how many times the coupon can be used.
    /// </summary>
    /// <default>10</default>
    [Range(2, int.MaxValue, ErrorMessage = "UsageLimit must be greater than 1.")]
    public int UsageLimit { get; set; }

    /// <summary>
    /// Gets or sets the count of how many times the coupon has been used.
    /// </summary>
    public int UsageCount { get; set; }

    /// <summary>
    /// Gets or sets the date when the coupon was created.
    /// </summary>
    /// <example>2024-06-15T14:30:00</example>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the duration type of the coupon, indicating whether it can be used once, repeatedly, or indefinitely.
    /// </summary>
    /// <example>repeating</example>
    /// <default>repeating</default>
    public string Duration { get; set; } // "once", "repeating", "forever"

    /// <summary>
    /// Gets or sets the duration in months for which the coupon is valid, if the duration type is "repeating".
    /// </summary>
    /// <default>3</default>
    [Range(2, int.MaxValue, ErrorMessage = "Duration must be greater than 1.")]
    public int DurationInMonths { get; set; } = 1;
  }

}
