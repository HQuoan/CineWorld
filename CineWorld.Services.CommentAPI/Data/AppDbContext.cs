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
            UserId = "4986f53c-a2db-4b71-80df-404bcad5413a",
            MovieId = 1,
             
            CommentContent = "This movie is amazing! I loved the characters and the plot.",

        },
        new Comment
        {
            CommentId = 2,
            CommentParentId = 1,
            UserId = "4986f53c-a2db-4b71-80df-404bcad5413a",
            MovieId = 1,
           
            CommentContent = "I agree, the movie was fantastic, especially the special effects!",

        },
        new Comment
        {
            CommentId = 3,
            CommentParentId = null,
            UserId = "4986f53c-a2db-4b71-80df-404bcad5413a",
            MovieId = 2,
            
            CommentContent = "Episode 1 had a slow start, but it picked up towards the end!",

        },
        new Comment
        {
            CommentId = 4,
            CommentParentId = 3,
            UserId = "4986f53c-a2db-4b71-80df-404bcad5413a",
            MovieId = 2,
           
            CommentContent = "Totally agree, the ending was the best part!",

        },
        new Comment
        {
            CommentId = 5,
            CommentParentId = null,
            UserId = "4986f53c-a2db-4b71-80df-404bcad5413a",
            MovieId = 3,
           
            CommentContent = "I didn't enjoy this movie as much as the previous ones. The pacing was off.",

        });




        }
    }
}
