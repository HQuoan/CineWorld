
namespace CineWorld.Services.MembershipAPI.APIFeatures
{
  public class MemberShipQueryParameters : BaseQueryParameters
  {
    public string? UserEmail { get; set; }

    /// <summary>
    /// Specifies the property name for sorting. 
    /// Valid values: "CategoryId, Name, Slug, Status".
    /// Use "-" prefix for descending order (e.g., "-Name" for descending by Name).
    /// </summary>
    public new string? OrderBy { get; set; }
  }
}
