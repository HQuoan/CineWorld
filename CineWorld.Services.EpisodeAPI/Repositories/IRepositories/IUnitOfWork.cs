namespace CineWorld.Services.EpisodeAPI.Repositories.IRepositories
{
  public interface IUnitOfWork
  {
    IEpisodeRepository Episode { get; }
    IServerRepository Server { get; }
    Task SaveAsync();
  }
}
