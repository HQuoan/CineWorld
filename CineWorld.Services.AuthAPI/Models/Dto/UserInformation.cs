namespace CineWorld.Services.AuthAPI.Models.Dto
{
  /// <summary>
  /// Represents the user information to be updated in the system.
  /// </summary>
  public class UserInformation
  {
    /// <summary>
    /// The unique identifier of the user.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// The full name of the user.
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// The URL or path to the user's avatar image (optional).
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// The gender of the user (e.g., Male, Female, Other).
    /// </summary>
    public string Gender { get; set; }

    /// <summary>
    /// The user's date of birth.
    /// </summary>
    public DateTime DateOfBirth { get; set; }
  }
}
