using CineWorld.Services.ReactionAPI.Models.Common;
using CineWorld.Services.ReactionAPI.Models.Dtos.WatchHistory;
using CineWorld.Services.ReactionAPI.Models.ReqParams;

namespace CineWorld.Services.ReactionAPI.Services.Interface
{
    public interface IWatchHistoryService
    {
        Task<bool> AddWatchHistoryAsync(string userId, WatchHistoryDTO watchHistory);
        Task<bool> DeleteWatchHistoryAsync(string userId, int watchHistoryId);
        Task<bool> DeleteAllWatchHistoriesAsync(string userId);
        Task<PagedList<WatchHistoryDTO>> GetHistoryByUserId (string userId, WatchHistoryParam reqParams);

    }
}
