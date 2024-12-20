using CineWorld.Services.ReactionAPI.Constants;
using CineWorld.Services.ReactionAPI.Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineWorld.Services.ReactionAPI.Models.Entities
{
    [Table(TableName.COMMENT)]
    public class Comment : EntityBase
    {
        [Key]
        [Column("id")]
        public int CommentId { get; set; }
        [Column("parent_id")]
        public int? CommentParentId { get; set; }
        [Required]
        [Column("user_id")]
        public string UserId { get; set; }
        [Column("movie_id")]
        public int MovieId { get; set; }
        [Required]
        [Column("comment_content")]
        public string CommentContent { get; set; }


    }
}
