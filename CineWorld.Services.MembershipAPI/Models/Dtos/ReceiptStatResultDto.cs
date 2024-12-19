namespace CineWorld.Services.MembershipAPI.Models.Dtos
{
  public class ReceiptStatResultDto
  {
    public int PackageId { get; set; }
    public int TermInMonths { get; set; }
    public decimal PackagePrice { get; set; }
    public int Count { get; set; }
    public decimal Total => PackagePrice * Count;
  }
}
