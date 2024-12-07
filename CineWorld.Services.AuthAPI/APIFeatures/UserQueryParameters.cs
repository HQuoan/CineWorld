
namespace CineWorld.Services.AuthAPI.APIFeatures
{
  public class UserQueryParameters : BaseQueryParameters
  {
    public  string? Email { get; set; }
    public string? FullName { get; set; }
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Role { get; set; }
    /// <summary>
    /// Specifies the property name for sorting. 
    /// Valid values: "FullName, Email, Role ".
    /// Use "-" prefix for descending order (e.g., "-Name" for descending by Name).
    /// </summary>
    public new string? OrderBy { get; set; }
  }


}
