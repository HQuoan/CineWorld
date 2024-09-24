using CineWorld.Services.AuthAPI.Utilities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineWorld.Services.AuthAPI.Models
{
  public class ApplicationUser : IdentityUser
  {
    public string Name { get; set; }
    public Gender Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    [NotMapped]
    public string? Role { get; set; }
  }
}
