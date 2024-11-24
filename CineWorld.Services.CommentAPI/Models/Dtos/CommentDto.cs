namespace CineWorld.Services.CommentAPI.Models.Dtos
{
    public class CommentDto
    {

        public int CommentId { get; set; }
        public int? CommentParentId { get; set; }

        public string UserId { get; set; }
        public string UserName { get; set; } = string.Empty;

        public int MovieId { get; set; }
        

        public string CommentContent { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
