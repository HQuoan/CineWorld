using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.MovieAPI.Attributes
{
  public class DayOfWeekValidationAttribute : ValidationAttribute
  {
    private static readonly string[] ValidDaysOfWeek = { "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "Chủ Nhật" };

    public override bool IsValid(object? value)
    {
      if (value == null)
      {
        return true; // hoặc false, tùy vào yêu cầu của bạn
      }

      var showTimesDetails = value.ToString();

      // Kiểm tra nếu chuỗi chứa ít nhất một trong các ngày hợp lệ
      return ValidDaysOfWeek.All(day => showTimesDetails.Contains(day));
    }

    public override string FormatErrorMessage(string name)
    {
      return $"{name} phải chứa ít nhất một ngày trong tuần (Thứ 2, Thứ 3, Thứ 4, Thứ 5, Thứ 6, Thứ 7, Chủ nhật).";
    }
  }
}
