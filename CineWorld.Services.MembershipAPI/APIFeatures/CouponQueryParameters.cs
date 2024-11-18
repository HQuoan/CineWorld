
namespace CineWorld.Services.MembershipAPI.APIFeatures
{
  public class CouponQueryParameters : BaseQueryParameters
  {
    public string? CouponCode { get; set; }
    public decimal? DiscountAmount { get; set; }
    public int? DurationInMonths { get; set; }
    public int? UsageLimit { get; set; }
    public int? UsageCount { get; set; }
    
    public bool? IsActive { get; set; }

    /// <summary>
    /// Specifies the property name for sorting. 
    /// Valid values: "CategoryId, Name, Slug, Status".
    /// Use "-" prefix for descending order (e.g., "-Name" for descending by Name).
    /// </summary>
    public new string? OrderBy { get; set; }
  }
}
