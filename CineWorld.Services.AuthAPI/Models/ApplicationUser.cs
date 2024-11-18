using CineWorld.Services.AuthAPI.Utilities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineWorld.Services.AuthAPI.Models
{
  public class ApplicationUser : IdentityUser
  {
    [Required]
    [EmailAddress]
    public override string Email { get; set; }
    [Required]
    public string FullName { get; set; }
    public string? Avatar { get; set; }
    [Required]
    [RegularExpression("^(Male|Female|Other)$", ErrorMessage = "Gender must be Male, Female, or Other.")]
    public string Gender { get; set; }
    [Required]
    public DateTime DateOfBirth { get; set; }
    [NotMapped]
    public string? Role { get; set; }
  }
}
