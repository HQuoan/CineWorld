namespace CineWorld.Services.ReactionAPI.Repositories.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IFavoriteRepository UserFavorites { get; }
        IRateRepository UserRates { get; }
        IWatchHistoryRepository WatchHistories { get; }
        ICommentRepository Comments { get; }
        Task<int> CompleteAsync(CancellationToken cancellationToken = default);
    }
}
