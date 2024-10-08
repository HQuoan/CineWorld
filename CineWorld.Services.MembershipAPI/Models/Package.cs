using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.MembershipAPI.Models
{
  public class Package
  {
    [Key]
    public int PackageId { get; set; }
    [Required]
    public string Name { get; set; }
    public string Description { get; set; }
    [Required]
    [Range(10, int.MaxValue)]
    public decimal Price { get; set; }
    [Required]

    public int TermInMonths { get; set; }
    public string Currency { get; set; } = "USD";
    public bool Status { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
  }
}
