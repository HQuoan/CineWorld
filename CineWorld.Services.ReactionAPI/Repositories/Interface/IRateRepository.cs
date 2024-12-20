using CineWorld.Services.ReactionAPI.Models.Entities;
using CineWorld.Services.ReactionAPI.Repositories.Generic_Repository;

namespace CineWorld.Services.ReactionAPI.Repositories.Interface
{
    public interface IRateRepository : IGenericRepository<UserRate>
    {
        IEnumerable<UserRate> GetRatingsByMovieId(int movieId);
        Task<UserRate> GetRateAsync(int movieId, string userId);
        //void AddRate(UserRate rating);
        //void UpdateRate(UserRate rating);
    }
}
