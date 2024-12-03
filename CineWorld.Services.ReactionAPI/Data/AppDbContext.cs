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
                
                
            );

            // Dữ liệu mẫu cho bảng UserRatess
            modelBuilder.Entity<UserRates>().HasData(
                
               
            );

        }

    }
}
