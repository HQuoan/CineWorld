using CineWorld.Services.ReactionAPI.Models.Dtos.UserFavorite;

namespace CineWorld.Services.ReactionAPI.Models.Dtos
{
    public class MovieResponseDto
    {
        public object Pagination { get; set; } // có thể là null hoặc kiểu dữ liệu khác tùy vào cấu trúc của bạn
        public List<ResponseFavoriteDTO> Result { get; set; } // Danh sách các bộ phim
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

    }
}
