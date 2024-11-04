using CineWorld.Services.CommentAPI.Models;

using Microsoft.EntityFrameworkCore;

namespace CineWorld.Services.CommentAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Comment> Comments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Comment>().HasData(
        new Comment
        {
            CommentId = 1,
            CommentParentId = null, // Comment gốc
            UserId = "3a5a3321-ebbe-49ac-b3aa-0466a2072515",
            MovieId = 1,
            EpisodeId = null,       // Không có Episode, bình luận chung cho movie
            CommentContent = "This movie is amazing! I loved the characters and the plot.",

        },
        new Comment
        {
            CommentId = 2,
            CommentParentId = 1,
            UserId = "af4f152c-5cdd-4027-8024-28919b019a2e",
            MovieId = 1,
            EpisodeId = null,
            CommentContent = "I agree, the movie was fantastic, especially the special effects!",

        },
        new Comment
        {
            CommentId = 3,
            CommentParentId = null,
            UserId = "3a5a3321-ebbe-49ac-b3aa-0466a2072515",
            MovieId = 2,
            EpisodeId = 1,
            CommentContent = "Episode 1 had a slow start, but it picked up towards the end!",

        },
        new Comment
        {
            CommentId = 4,
            CommentParentId = 3,
            UserId = "af4f152c-5cdd-4027-8024-28919b019a2e",
            MovieId = 2,
            EpisodeId = 1,
            CommentContent = "Totally agree, the ending was the best part!",

        },
        new Comment
        {
            CommentId = 5,
            CommentParentId = null,
            UserId = "b736c9ed-4b4f-4f75-be82-186c25a69e4e",
            MovieId = 3,
            EpisodeId = null,
            CommentContent = "I didn't enjoy this movie as much as the previous ones. The pacing was off.",

        });




        }
    }
}
