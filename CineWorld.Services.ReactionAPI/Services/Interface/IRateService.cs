using CineWorld.Services.ReactionAPI.Models.Dtos.UserRate;

namespace CineWorld.Services.ReactionAPI.Services.Interface
{
    public interface IRateService
    {
        ResponseRatingDTO GetAverageRatingAsync(int movieId);
        Task<UserRateDTO> GetRateAsync(int movieId, string userId);
        Task<bool> AddRateAsync(string userId, UserRateDTO rating);
        Task<bool> UpdateRateAsync(string userId, UserRateDTO rating);
    }

}
