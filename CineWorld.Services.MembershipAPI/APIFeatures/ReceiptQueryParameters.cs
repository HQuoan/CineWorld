
using CineWorld.Services.MembershipAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.MembershipAPI.APIFeatures
{
  public class ReceiptQueryParameters : BaseQueryParameters
  {
    public string? UserId { get; set; }
    public string? Email { get; set; }
    public int? PackageId { get; set; }
    public string? CouponCode { get; set; }
    public decimal? PackagePrice { get; set; }
    public int? TermInMonths { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? Status { get; set; }

    /// <summary>
    /// Specifies the property name for sorting. 
    /// Valid values: "CategoryId, Name, Slug, Status".
    /// Use "-" prefix for descending order (e.g., "-Name" for descending by Name).
    /// </summary>
    public new string? OrderBy { get; set; }
  }
}
