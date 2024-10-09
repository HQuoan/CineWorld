using CineWorld.Services.AuthAPI.Utilities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineWorld.Services.AuthAPI.Models
{
  public class ApplicationUser : IdentityUser
  {
    [Required]
    public string Name { get; set; }
    [Required]
    [RegularExpression("^(Male|Female|Other)$", ErrorMessage = "Gender must be Male, Female, or Other.")]
    public string Gender { get; set; }
    [Required]
    public DateTime DateOfBirth { get; set; }
    public DateTime? MembershipEndDate { get; set; }
    [NotMapped]
    public string? Role { get; set; }
  }
}
