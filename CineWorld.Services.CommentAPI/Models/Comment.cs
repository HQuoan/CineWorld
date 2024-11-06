using System;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace CineWorld.Services.CommentAPI.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public int? CommentParentId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int MovieId { get; set; }
        public int? EpisodeId { get; set; }
        [Required]
        public string CommentContent { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;


    }
}
