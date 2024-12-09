namespace CineWorld.Services.AuthAPI.Models.Dto
{
  /// <summary>
  /// Data transfer object representing user information.
  /// </summary>
  public class UserDto
  {
    /// <summary>
    /// The unique identifier of the user.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// The email address of the user.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// The full name of the user.
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// The URL or path to the user's avatar image. Optional.
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// The gender of the user (e.g., Male, Female, Other).
    /// </summary>
    public string Gender { get; set; }

    /// <summary>
    /// The date of birth of the user.
    /// </summary>
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// The role(s) assigned to the user. Optional.
    /// </summary>
    public string? Role { get; set; }

    public DateTime CreatedDate { get; set; }
  }
}
