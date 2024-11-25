using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.MembershipAPI.Models.Dtos
{
  /// <summary>
  /// Represents a membership subscription for a user, including information on subscription dates and status.
  /// </summary>
  public class MemberShipDto
  {
    /// <summary>
    /// Gets or sets the unique identifier for the membership.
    /// </summary>
    public int MemberShipId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the user associated with the membership.
    /// </summary>
    /// <example>"12345"</example>
    [Required]
    public string UserId { get; set; }

    /// <summary>
    /// Gets or sets the email address of the user associated with the membership.
    /// </summary>
    /// <example>"user@example.com"</example>
    [EmailAddress]
    public string UserEmail { get; set; }

    /// <summary>
    /// Gets or sets the date when the user first subscribed to the membership.
    /// </summary>
    /// <example>2024-01-01T12:00:00</example>
    public DateTime FirstSubscriptionDate { get; set; } = DateTime.UtcNow; // The first subscription date

    /// <summary>
    /// Gets or sets the date when the renewal period starts for the membership.
    /// </summary>
    /// <example>2024-06-01T12:00:00</example>
    public DateTime RenewalStartDate { get; set; } = DateTime.UtcNow; // The renewal start date

    /// <summary>
    /// Gets or sets the date when the membership details were last updated.
    /// </summary>
    /// <example>2024-06-15T15:00:00</example>
    public DateTime LastUpdatedDate { get; set; } = DateTime.UtcNow; // The last updated date

    /// <summary>
    /// Gets or sets the expiration date of the membership.
    /// </summary>
    /// <example>2025-01-01T12:00:00</example>
    [Required]
    public DateTime ExpirationDate { get; set; } = DateTime.UtcNow; // The expiration date
  }
}
