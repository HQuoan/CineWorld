using CineWorld.Services.AuthAPI.Utilities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.AuthAPI.Models.Dto
{
  public class RegistrationRequestDto
  {
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; }
    [Required]
    public string FullName { get; set; }
    [Required(ErrorMessage = "Password is required.")]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character."
    )]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters.")]
    [DefaultValue("User@123")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Please confirm your password.")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    [DefaultValue("User@123")]
    public string ConfirmPassword { get; set; }
    [Required]
    [RegularExpression("^(Male|Female|Other)$", ErrorMessage = "Gender must be Male, Female, or Other.")]
    [DefaultValue("Male")]
    public string Gender { get; set; }
    [Required]
    public DateTime DateOfBirth { get; set; }
  }
}
