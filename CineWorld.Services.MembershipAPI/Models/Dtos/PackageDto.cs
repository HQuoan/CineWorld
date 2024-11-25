using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.MembershipAPI.Models.Dtos
{
  /// <summary>
  /// Represents a package in the membership system, including details like price, term, and status.
  /// </summary>
  public class PackageDto
  {
    /// <summary>
    /// Gets or sets the unique identifier for the package.
    /// </summary>
    public int PackageId { get; set; }

    /// <summary>
    /// Gets or sets the name of the package. This field is required.
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the package.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the price of the package. This field is required and must be greater than or equal to 10.
    /// Default value is 10.
    /// </summary>
    [Required]
    [Range(10, int.MaxValue)]
    [DefaultValue(10)]
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the duration of the package in months. This field is required and must be between 1 and 24 months.
    /// Default value is 1 month.
    /// </summary>
    [Required]
    [Range(1, 24)]
    [DefaultValue(1)]
    public int TermInMonths { get; set; }

    /// <summary>
    /// Gets or sets the currency for the package price. Default is "USD".
    /// </summary>
    [DefaultValue("USD")]
    public string Currency { get; set; }

    /// <summary>
    /// Gets or sets the status of the package. The default is true (active).
    /// </summary>
    public bool Status { get; set; } = true;

    /// <summary>
    /// Gets or sets the date when the package was created.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the date when the package was last updated.
    /// </summary>
    public DateTime UpdatedDate { get; set; }
  }
}
