namespace CineWorld.Services.MembershipAPI.Repositories.IRepositories
{
  public interface IUnitOfWork
  {
    ICouponRepository Coupon { get; }
    IPackageRepository Package { get; }
    IReceiptRepository Receipt { get; }
    IMemberShipRepository MemberShip { get; }
    Task SaveAsync();
  }
}
