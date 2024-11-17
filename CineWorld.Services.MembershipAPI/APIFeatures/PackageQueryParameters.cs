
namespace CineWorld.Services.MembershipAPI.APIFeatures
{
  public class PackageQueryParameters : BaseQueryParameters
  {
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public int? TermInMonths { get; set; }
    public bool? Status { get; set; }

    /// <summary>
    /// Specifies the property name for sorting. 
    /// Valid values: "CategoryId, Name, Slug, Status".
    /// Use "-" prefix for descending order (e.g., "-Name" for descending by Name).
    /// </summary>
    public new string? OrderBy { get; set; }
  }
}
