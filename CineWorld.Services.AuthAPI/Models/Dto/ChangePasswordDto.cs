using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.AuthAPI.Models.Dto
{
  /// <summary>
  /// Data transfer object for changing a user's password.
  /// </summary>
  public class ChangePasswordDto
  {
    /// <summary>
    /// The user's current password. This is required to verify the user's identity.
    /// </summary>
    public string CurrentPassword { get; set; }

    /// <summary>
    /// The new password that the user wants to set.
    /// </summary>
    public string NewPassword { get; set; }

    /// <summary>
    /// Confirmation of the new password to ensure accuracy.
    /// Must match the value of NewPassword/>.
    /// </summary>
    [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmNewPassword { get; set; }
  }
}
