using CineWorld.Services.ReactionAPI.Models.Common;
using CineWorld.Services.ReactionAPI.Models.Entities;
using CineWorld.Services.ReactionAPI.Models.ReqParams;
using CineWorld.Services.ReactionAPI.Repositories.Generic_Repository;

namespace CineWorld.Services.ReactionAPI.Repositories.Interface
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        Task<PagedList<Comment>> GetCommentByMovieId(int movieId, CommentParam reqParams);
    }
}
