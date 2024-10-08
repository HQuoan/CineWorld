using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.MembershipAPI.Models.Dtos
{
  public class PackageDto
  {
    public int PackageId { get; set; }
    [Required]
    public string Name { get; set; }
    public string Description { get; set; }
    [Required]
    public decimal Price { get; set; }
    [Required]
    [Range(1, 24)]
    public int TermInMonths { get; set; }
    public string Currency { get; set; } = "USD";
    public bool Status { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
  }
}
