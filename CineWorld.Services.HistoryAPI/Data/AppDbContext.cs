using CineWorld.Services.HistoryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CineWorld.Services.HistoryAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<WatchHistory> watchHistories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<WatchHistory>().HasData(
           //new WatchHistory
           //{
           //    Id = 1,
           //    UserId = "user123",
           //    MovieId = 1,
           //    EpisodeId = 2,
           //    MovieUrl = "seedTable.com",
           //    WatchedDuration = TimeSpan.FromMinutes(20), 
           //    LastWatched = DateTime.Parse("2024-11-01T10:30:00Z")
           //},
           //new WatchHistory
           //{
           //    Id = 2,
           //    UserId = "user456",
           //    MovieId = 2,
           //    EpisodeId = 1,
           //    MovieUrl = "seedTable.com",
           //    WatchedDuration = TimeSpan.FromMinutes(15), 
           //    LastWatched = DateTime.Parse("2024-11-02T11:00:00Z")
           //},
           //new WatchHistory
           //{
           //    Id = 3,
           //    UserId = "user789",
           //    MovieId = 3,
           //    EpisodeId = 3,
           //    MovieUrl = "seedTable.com",
           //    WatchedDuration = TimeSpan.FromMinutes(5), 
           //    LastWatched = DateTime.Parse("2024-11-03T14:45:00Z")
           //},
           //new WatchHistory
           //{
           //    Id = 4,
           //    UserId = "user123",
           //    MovieId = 4,
           //    EpisodeId = 1,
           //    MovieUrl = "seedTable.com",
           //    WatchedDuration = TimeSpan.FromMinutes(30), 
           //    LastWatched = DateTime.Parse("2024-11-04T16:00:00Z")
           //},
           //new WatchHistory
           //{
           //    Id = 5,
           //    UserId = "user456",
           //    MovieId = 5,
           //    EpisodeId = 2,
           //    MovieUrl = "seedTable.com",
           //    WatchedDuration = TimeSpan.FromMinutes(10), 
           //    LastWatched = DateTime.Parse("2024-11-05T18:15:00Z")
           //}
       );

        }
    }
}
