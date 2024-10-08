using System.ComponentModel;
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
    [Range(10, int.MaxValue)]
    [DefaultValue(10)]
    public decimal Price { get; set; }
    [Required]
    [Range(1, 24)]
    [DefaultValue(1)]
    public int TermInMonths { get; set; }
    [DefaultValue("USD")]
    public string Currency { get; set; }
    public bool Status { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
  }
}
