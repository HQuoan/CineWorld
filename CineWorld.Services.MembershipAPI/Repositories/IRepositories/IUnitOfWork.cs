namespace CineWorld.Services.MovieAPI.Repositories.IRepositories
{
  public interface IUnitOfWork
  {
    ICouponRepository Coupon { get; }
    IPackageRepository Package { get; }
    IReceiptRepository Receipt { get; }
    Task SaveAsync();
  }
}
