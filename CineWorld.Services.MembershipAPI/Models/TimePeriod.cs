using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.MembershipAPI.Models
{
  public class TimePeriod : IValidatableObject
  {
    public DateTime From { get; set; } = DateTime.MinValue;

    public DateTime To { get; set; } = DateTime.UtcNow;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
      if (To <= From)
      {
        yield return new ValidationResult(
            "The 'To' date must be greater than the 'From' date.",
            new[] { nameof(To) }
        );
      }
    }
  }

}
