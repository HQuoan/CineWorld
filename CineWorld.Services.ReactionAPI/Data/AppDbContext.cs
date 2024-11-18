using CineWorld.Services.ReactionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CineWorld.Services.ReactionAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<UserRates> UserRates { get; set; }
        public DbSet<UserFavorites> UserFavorites { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserFavorites>()
       .HasKey(uf => new { uf.UserId, uf.MovieId });
            // Dữ liệu mẫu cho bảng UserFavorites
            modelBuilder.Entity<UserFavorites>().HasData(
                new UserFavorites { UserId = "user1", MovieId = 101, FavoritedAt = new DateTime(2024, 11, 1, 10, 30, 0) },
                new UserFavorites { UserId = "user1", MovieId = 102, FavoritedAt = new DateTime(2024, 11, 1, 11, 0, 0) },
                new UserFavorites { UserId = "user2", MovieId = 101, FavoritedAt = new DateTime(2024, 11, 1, 12, 0, 0) },
                new UserFavorites { UserId = "user3", MovieId = 103, FavoritedAt = new DateTime(2024, 11, 2, 9, 15, 0) },
                new UserFavorites { UserId = "user1", MovieId = 103, FavoritedAt = new DateTime(2024, 11, 2, 10, 0, 0) }
            );

            // Dữ liệu mẫu cho bảng UserRatess
            modelBuilder.Entity<UserRates>().HasData(
                new UserRates { RatingId = 1, UserId = "user1", EpisodeId = 101, RatingValue = 4, RatedAt = new DateTime(2024, 11, 1, 10, 45, 0) },
                new UserRates { RatingId = 2, UserId = "user2", EpisodeId = 101, RatingValue = 5, RatedAt = new DateTime(2024, 11, 1, 12, 30, 0) },
                new UserRates { RatingId = 3, UserId = "user1", EpisodeId = 102, RatingValue = 3, RatedAt = new DateTime(2024, 11, 1, 11, 15, 0) },
                new UserRates { RatingId = 4, UserId = "user3", EpisodeId = 103, RatingValue = 2, RatedAt = new DateTime(2024, 11, 2, 9, 30, 0) },
                new UserRates { RatingId = 5, UserId = "user1", EpisodeId = 103, RatingValue = 4, RatedAt = new DateTime(2024, 11, 2, 10, 5, 0) }
            );

        }

    }
}
