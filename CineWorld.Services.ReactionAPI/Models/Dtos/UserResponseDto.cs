using CineWorld.Services.ReactionAPI.Models.Dtos.UserFavorite;

namespace CineWorld.Services.ReactionAPI.Models.Dto
{
    public class UserResponseDto
    {
        public List<UserCommentDTO> Result { get; set; } 
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
