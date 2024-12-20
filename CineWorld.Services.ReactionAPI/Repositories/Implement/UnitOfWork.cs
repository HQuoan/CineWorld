using CineWorld.Services.ReactionAPI.Data;
using CineWorld.Services.ReactionAPI.Repositories.Interface;

namespace CineWorld.Services.ReactionAPI.Repositories.Implement
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public IFavoriteRepository UserFavorites { get; private set; }
        public IRateRepository UserRates { get; private set; }
        public IWatchHistoryRepository WatchHistories { get; private set; }
        public ICommentRepository Comments { get; private set; }
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            UserFavorites = new FavoriteRepository(context);
            UserRates = new RateRepository(context);
            WatchHistories = new WatchHistoryRepository(context);
            Comments = new CommentRepository(context);

        }
        public async Task<int> CompleteAsync(CancellationToken cancellationToken=default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
