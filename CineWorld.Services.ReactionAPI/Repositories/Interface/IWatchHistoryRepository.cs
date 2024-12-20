using CineWorld.Services.ReactionAPI.Models.Common;
using CineWorld.Services.ReactionAPI.Models.Entities;
using CineWorld.Services.ReactionAPI.Models.ReqParams;
using CineWorld.Services.ReactionAPI.Repositories.Generic_Repository;

namespace CineWorld.Services.ReactionAPI.Repositories.Interface
{
    public interface IWatchHistoryRepository : IGenericRepository<WatchHistory>
    {
        Task<bool> IsMovieWatchedAsync(string userId, int movieId, int episodeId);
        Task<PagedList<WatchHistory>> GetFavoriteByUserId(string userId, WatchHistoryParam reqParams);
        void UpdateWatchHistory(WatchHistory watchHistory);
    }
}
