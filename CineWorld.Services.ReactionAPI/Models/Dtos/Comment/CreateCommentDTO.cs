namespace CineWorld.Services.ReactionAPI.Models.Dtos.Comment
{
    public class CreateCommentDTO
    {
        public int CommentId { get; set; }
        public int? CommentParentId { get; set; }
        public int MovieId { get; set; }
        public string CommentContent { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
