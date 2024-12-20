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
            FullName = "Admin",
            Avatar = "https://cineworld-user-avatars.s3.amazonaws.com/users/4986f53c-a2db-4b71-80df-404bcad5413a/c7a45e1d-6b3b-4ef7-bd46-f28b32578dc1_bx170468-kD2X9O2XM9KH.jpg",


            CommentContent = "This movie is amazing! I loved the characters and the plot.",

        },
        new Comment
        {
            CommentId = 2,
            CommentParentId = 1,
            UserId = "4986f53c-a2db-4b71-80df-404bcad5413a",
            MovieId = 1,
            FullName = "Admin",
            Avatar = "https://cineworld-user-avatars.s3.amazonaws.com/users/4986f53c-a2db-4b71-80df-404bcad5413a/c7a45e1d-6b3b-4ef7-bd46-f28b32578dc1_bx170468-kD2X9O2XM9KH.jpg",
            CommentContent = "I agree, the movie was fantastic, especially the special effects!",

        },
        new Comment
        {
            CommentId = 3,
            CommentParentId = null,
            UserId = "4986f53c-a2db-4b71-80df-404bcad5413a",
            MovieId = 2,
            FullName = "Admin",
            Avatar = "https://cineworld-user-avatars.s3.amazonaws.com/users/4986f53c-a2db-4b71-80df-404bcad5413a/c7a45e1d-6b3b-4ef7-bd46-f28b32578dc1_bx170468-kD2X9O2XM9KH.jpg",

            CommentContent = "Episode 1 had a slow start, but it picked up towards the end!",

        });




        }
    }
}
