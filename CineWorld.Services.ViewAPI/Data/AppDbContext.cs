using CineWorld.Services.ViewAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CineWorld.Services.ViewAPI.Data
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    public DbSet<View> Views { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // Đánh index View 
      modelBuilder.Entity<View>()
         .HasIndex(c => c.IpAddress);
      modelBuilder.Entity<View>()
          .HasIndex(c => c.UserId);
      modelBuilder.Entity<View>()
          .HasIndex(c => c.MovieId);
      modelBuilder.Entity<View>()
         .HasIndex(c => c.EpisodeId);
      modelBuilder.Entity<View>()
         .HasIndex(c => c.ViewDate);
    }
  }
}
