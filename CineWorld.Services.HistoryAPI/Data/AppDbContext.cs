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
           new WatchHistory
           {
               Id = 1,
               UserId = "user123",

               EpisodeId = 2,
               WatchedDuration = TimeSpan.FromMinutes(20), 
               LastWatched = DateTime.Parse("2024-11-01T10:30:00Z")
           },
           new WatchHistory
           {
               Id = 2,
               UserId = "user456",

               EpisodeId = 1,
               WatchedDuration = TimeSpan.FromMinutes(15), 
               LastWatched = DateTime.Parse("2024-11-02T11:00:00Z")
           },
           new WatchHistory
           {
               Id = 3,
               UserId = "user789",

               EpisodeId = 3,
               WatchedDuration = TimeSpan.FromMinutes(5), 
               LastWatched = DateTime.Parse("2024-11-03T14:45:00Z")
           },
           new WatchHistory
           {
               Id = 4,
               UserId = "user123",

               EpisodeId = 1,
               WatchedDuration = TimeSpan.FromMinutes(30), 
               LastWatched = DateTime.Parse("2024-11-04T16:00:00Z")
           },
           new WatchHistory
           {
               Id = 5,
               UserId = "user456",

               EpisodeId = 2,
               WatchedDuration = TimeSpan.FromMinutes(10), 
               LastWatched = DateTime.Parse("2024-11-05T18:15:00Z")
           }
       );

        }
    }
}
