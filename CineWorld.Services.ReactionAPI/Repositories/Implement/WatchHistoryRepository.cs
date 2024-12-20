using CineWorld.Services.ReactionAPI.Data;
using CineWorld.Services.ReactionAPI.Extensions;
using CineWorld.Services.ReactionAPI.Models.Common;
using CineWorld.Services.ReactionAPI.Models.Entities;
using CineWorld.Services.ReactionAPI.Models.ReqParams;
using CineWorld.Services.ReactionAPI.Repositories.Generic_Repository;
using CineWorld.Services.ReactionAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CineWorld.Services.ReactionAPI.Repositories.Implement
{
    public class WatchHistoryRepository : GenericRepository<WatchHistory>, IWatchHistoryRepository
    {
        public WatchHistoryRepository(AppDbContext dbcontext) : base(dbcontext)
        {
        }
        public async Task<bool> IsMovieWatchedAsync(string userId, int movieId, int episodeId)
        {
            return await _dbcontext.WatchHistories.AnyAsync(p => p.UserId == userId && p.MovieId == movieId && p.EpisodeId == episodeId);
        }

        public async Task<PagedList<WatchHistory>> GetFavoriteByUserId(string userId, WatchHistoryParam reqParams)
        {
            var entity = _dbcontext.WatchHistories.Where(history => history.UserId == userId);
            return await entity.ToPagedList<WatchHistory>(reqParams.PageNumber, reqParams.PageSize);
        }

        public void UpdateWatchHistory(WatchHistory watchHistory)
        {
            WatchHistory? existingEntity = _dbcontext.WatchHistories
                .FirstOrDefault(wh => wh.UserId == watchHistory.UserId
                && wh.MovieId == watchHistory.MovieId
                && wh.EpisodeId == watchHistory.EpisodeId);
            if (existingEntity != null)
            {
                existingEntity.WatchedDuration = watchHistory.WatchedDuration;
                existingEntity.LastWatched = watchHistory.LastWatched;

                _dbcontext.WatchHistories.Update(existingEntity);
            }
        }
    }
}
