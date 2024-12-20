using CineWorld.Services.ReactionAPI.Data;
using CineWorld.Services.ReactionAPI.Models.Entities;
using CineWorld.Services.ReactionAPI.Repositories.Generic_Repository;
using CineWorld.Services.ReactionAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CineWorld.Services.ReactionAPI.Repositories.Implement
{
    public class RateRepository : GenericRepository<UserRate>, IRateRepository
    {

        public RateRepository(AppDbContext context) : base(context)
        {

        }
        public IEnumerable<UserRate> GetRatingsByMovieId(int movieId)
        {
            var rating = _dbcontext.UserRates
                .Where(p => p.MovieId == movieId);
            return rating;
               
        }

        public async Task<UserRate> GetRateAsync(int movieId, string userId)
        {
            UserRate rate = await _dbcontext.UserRates.Where(p => p.MovieId == movieId && p.UserId == userId).FirstOrDefaultAsync();
            return rate;
        }

    }
}
