using CineWorld.Services.ReactionAPI.Models.Common;
using CineWorld.Services.ReactionAPI.Models.Dtos.Comment;
using CineWorld.Services.ReactionAPI.Models.Dtos.UserFavorite;
using CineWorld.Services.ReactionAPI.Models.ReqParams;

namespace CineWorld.Services.ReactionAPI.Services.Interface
{
    public interface ICommentService
    {
        Task<bool> AddCommentAsync(string userId, CreateCommentDTO comment);
        Task<bool> UpdateCommentAsync(string role, string userId, CreateCommentDTO commentDto);
        Task<bool> DeleteCommentAsync(string role, string userId, int CommentId);
        Task<PagedList<CommentDTO>> GetCommentByMovieIdAsync(int movieId, CommentParam reqParams);
    }
}
