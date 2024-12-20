using CineWorld.Services.ReactionAPI.Models.Common;
using CineWorld.Services.ReactionAPI.Models.Dtos.UserFavorite;
using CineWorld.Services.ReactionAPI.Models.Entities;
using CineWorld.Services.ReactionAPI.Models.ReqParams;
using CineWorld.Services.ReactionAPI.Repositories.Generic_Repository;

namespace CineWorld.Services.ReactionAPI.Services.Interface
{
    public interface IFavoriteService 
    {
        Task<bool> CheckFavoriteAsync(string userId, int movieId);
        Task<bool> AddFavoriteAsync(string userId, UserFavoriteDTO userFavorite);
        Task<bool> RemoveFavoriteAsync(string userId, UserFavoriteDTO userFavorite);
        Task<PagedList<ResponseFavoriteDTO>> GetFavoriteByUserId(string userId, FavoriteParam reqParams);
    } 
}
