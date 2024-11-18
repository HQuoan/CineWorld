using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.AuthAPI.Models.Dto
{
  public class ResetPasswordDto
  {
    public string Email { get; set; }
    public string Token { get; set; }
    public string NewPassword { get; set; }
    [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmNewPassword { get; set; }
  }

}
