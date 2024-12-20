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
    public class FavoriteRepository : GenericRepository<UserFavorite>, IFavoriteRepository
    {

        public FavoriteRepository(AppDbContext _context) : base(_context)
        {

        }
        public async Task<bool> CheckFavoriteAsync(string userId, int movieId)
        {
            bool isFavorited = await _dbcontext.UserFavorites
                                     .AnyAsync(p => p.MovieId == movieId && p.UserId == userId);
            return isFavorited;
        }
        public async Task<PagedList<UserFavorite>> GetFavoriteByUserId(string userId, FavoriteParam reqParams)
        {
            var entity = _dbcontext.UserFavorites.Where(p => p.UserId == userId);
            return await entity.ToPagedList(reqParams.PageNumber, reqParams.PageSize);
        }


    }
}
