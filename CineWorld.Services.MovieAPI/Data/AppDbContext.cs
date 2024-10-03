using CineWorld.Services.MovieAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CineWorld.Services.MovieAPI.Data
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Series> Series { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Episode> Episodes { get; set; }
    public DbSet<Server> Servers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // mặc định tạo CreatedDate khi tạo và không update được 
      modelBuilder.Entity<Episode>()
           .Property(m => m.CreatedDate)
           .HasDefaultValueSql("GETDATE()")
           .ValueGeneratedOnAdd()
           .Metadata.SetAfterSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore);

      modelBuilder.Entity<Episode>()
         .HasIndex(e => new { e.MovieId, e.EpisodeNumber })
         .IsUnique()
         .HasName("IX_Movie_EpisodeNumber");

      // mặc định tạo CreatedDate khi tạo và không update được 
      modelBuilder.Entity<Movie>()
           .Property(m => m.CreatedDate)
           .HasDefaultValueSql("GETDATE()") 
           .ValueGeneratedOnAdd()
           .Metadata.SetAfterSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore);

      // Cấu hình quan hệ 1-nhiều giữa Series và Movie
      modelBuilder.Entity<Movie>()
          .HasOne(m => m.Series) // 1 Movie có thể có 1 Series
          .WithMany(s => s.Movies) // 1 Series có nhiều Movie
          .HasForeignKey(m => m.SeriesId) // Movie có khóa ngoại SeriesId
          .IsRequired(false); // SeriesId không bắt buộc

      // Tạo Slug là unique
      modelBuilder.Entity<Category>()
           .HasIndex(c => c.Slug)
           .IsUnique();
      modelBuilder.Entity<Country>()
          .HasIndex(c => c.Slug)
          .IsUnique();
      modelBuilder.Entity<Series>()
          .HasIndex(c => c.Slug)
          .IsUnique();
      modelBuilder.Entity<Movie>()
          .HasIndex(c => c.Slug)
          .IsUnique();
      modelBuilder.Entity<Genre>()
          .HasIndex(c => c.Slug)
          .IsUnique();

    }

  }
}
