using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.MembershipAPI.Models.Dtos
{
  /// <summary>
  /// Represents a request object for Stripe payment session details.
  /// </summary>
  public class PaymentRequestDto
  {
    /// <summary>
    /// Gets or sets the unique ID of the receipt associated with this Stripe payment request.
    /// This is a required field.
    /// </summary>
    [Required]
    public int ReceiptId { get; set; }
    /// <summary>
    /// Gets or sets the URL for the payment session where the user can complete the payment.
    /// </summary>
    public string? PaymentSessionUrl { get; set; }
    /// <summary>
    /// Gets or sets the URL where the user will be redirected after successfully approving the payment.
    /// Default is a Google Drive URL.
    /// </summary>
    [DefaultValue("https://drive.google.com/file/d/1BjNcczy3hcsiLWNdzywwM8ay30MLJdyR/view?usp=sharing")]
    public string ApprovedUrl { get; set; }

    /// <summary>
    /// Gets or sets the URL where the user will be redirected if they cancel the payment.
    /// Default is a Google Drive URL.
    /// </summary>
    [DefaultValue("https://drive.google.com/file/d/1BjNcczy3hcsiLWNdzywwM8ay30MLJdyR/view?usp=sharing")]
    public string CancelUrl { get; set; }
  }
}
