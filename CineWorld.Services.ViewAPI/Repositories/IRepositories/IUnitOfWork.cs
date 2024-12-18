namespace CineWorld.Services.ViewAPI.Repositories.IRepositories
{
  public interface IUnitOfWork
  {
    IViewRepository View { get; }
    Task SaveAsync();
  }
}
