using CineWorld.Services.ReactionAPI.Data;
using CineWorld.Services.ReactionAPI.Extensions;
using CineWorld.Services.ReactionAPI.Models.Common;
using CineWorld.Services.ReactionAPI.Models.Entities;
using CineWorld.Services.ReactionAPI.Models.ReqParams;
using CineWorld.Services.ReactionAPI.Repositories.Generic_Repository;
using CineWorld.Services.ReactionAPI.Repositories.Interface;

namespace CineWorld.Services.ReactionAPI.Repositories.Implement
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        public CommentRepository(AppDbContext _context) : base(_context)
        {
        }

        public async Task<PagedList<Comment>> GetCommentByMovieId(int movieId, CommentParam reqParams)
        {
            var entity = _dbcontext.Comments.Where(p => p.MovieId == movieId);
            return await entity.ToPagedList(reqParams.PageNumber, reqParams.PageSize);
        }
    }
}
