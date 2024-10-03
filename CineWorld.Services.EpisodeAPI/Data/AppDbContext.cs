using CineWorld.Services.EpisodeAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CineWorld.Services.EpisodeAPI.Data
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
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
    }

  }
}
