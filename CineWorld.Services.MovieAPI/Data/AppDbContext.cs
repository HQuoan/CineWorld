using CineWorld.Services.MovieAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CineWorld.Services.MovieAPI.Data
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {

    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Series> Series { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Genre> Genres { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // Cấu hình mối quan hệ nhiều-nhiều giữa Movie và Genre
      modelBuilder.Entity<Movie>()
          .HasMany(m => m.Genres)
          .WithMany(g => g.Movies)
          .UsingEntity<Dictionary<string, object>>(
              "MovieGenre",  // Tên của bảng trung gian
              j => j.HasOne<Genre>().WithMany().HasForeignKey("GenreId"),
              j => j.HasOne<Movie>().WithMany().HasForeignKey("MovieId")
          );
    }

  }
}
