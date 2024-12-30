namespace CineWorld.Services.MovieAPI.Models.Dtos
{
  public class MemberShipDto
  {
    public int MemberShipId { get; set; }
    public string UserId { get; set; }
    public string UserEmail { get; set; }
    public DateTime FirstSubscriptionDate { get; set; } = DateTime.UtcNow;// Ngày đăng ký lần đầu
    public DateTime RenewalStartDate { get; set; } = DateTime.UtcNow; // Ngày bắt đầu gia hạn
    public DateTime LastUpdatedDate { get; set; } = DateTime.UtcNow;// Ngày cập nhật lần cuối
    public DateTime ExpirationDate { get; set; } = DateTime.UtcNow;// Ngày hết hạn
  }
}
