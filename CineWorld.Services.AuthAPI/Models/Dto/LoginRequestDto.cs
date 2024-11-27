using System.ComponentModel;

namespace CineWorld.Services.AuthAPI.Models.Dto
{
  /// <summary>
  /// Data transfer object for user login requests.
  /// </summary>
  public class LoginRequestDto
  {
    /// <summary>
    /// The email address of the user attempting to log in.
    /// </summary>
    [DefaultValue("admin@gmail.com")]
    public string Email { get; set; }

    /// <summary>
    /// The password associated with the user's account.
    /// </summary>
    [DefaultValue("Admin@123")]
    public string Password { get; set; }
  }
}
