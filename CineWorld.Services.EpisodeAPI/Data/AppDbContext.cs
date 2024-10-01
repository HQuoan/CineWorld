using CineWorld.Services.EpisodeAPI.Models;
using CineWorld.Services.MovieAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CineWorld.Services.EpisodeAPI.Data
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // Tạo Slug là unique
      modelBuilder.Entity<Episode>()
           .HasIndex(c => c.EpisodeNumber)
           .IsUnique();
    }

  }
}
