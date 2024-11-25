using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.AuthAPI.Models.Dto
{
  /// <summary>
  /// Data transfer object representing a request to reset a user's password.
  /// </summary>
  public class ResetPasswordDto
  {
    /// <summary>
    /// The email address of the user requesting the password reset.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// The token used to verify the password reset request.
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// The new password for the user account. 
    /// </summary>
    public string NewPassword { get; set; }

    /// <summary>
    /// The confirmation of the new password. Must match the new password.
    /// </summary>
    [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmNewPassword { get; set; }
  }
}
