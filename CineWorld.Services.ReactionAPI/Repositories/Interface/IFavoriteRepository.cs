using CineWorld.Services.ReactionAPI.Models.Common;
using CineWorld.Services.ReactionAPI.Models.Entities;
using CineWorld.Services.ReactionAPI.Models.ReqParams;
using CineWorld.Services.ReactionAPI.Repositories.Generic_Repository;

namespace CineWorld.Services.ReactionAPI.Repositories.Interface
{
    public interface IFavoriteRepository : IGenericRepository<UserFavorite>
    {
        Task<bool> CheckFavoriteAsync(string userId, int movieId);
        Task<PagedList<UserFavorite>> GetFavoriteByUserId(string userId, FavoriteParam reqParams);
        //Task AddFavoriteAsync(UserFavorite userFavorite);
        //Task RemoveFavoriteAsync(string userId, int movieId);
    }
}
