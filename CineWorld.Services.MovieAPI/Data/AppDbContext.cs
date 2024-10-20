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
    public DbSet<MovieGenre> MovieGenres { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // Seed to Categories
      string categoriesJson = System.IO.File.ReadAllText("Data/SeedData/categories.json");
      List<Category> categories = System.Text.Json.JsonSerializer.Deserialize<List<Category>>(categoriesJson);
      modelBuilder.Entity<Category>().HasData(categories.ToArray());

      // Seed to Countries
      string countriesJson = System.IO.File.ReadAllText("Data/SeedData/countries.json");
      List<Country> countries = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesJson);
      modelBuilder.Entity<Country>().HasData(countries.ToArray());

      // Seed to Genres
      string genresJson = System.IO.File.ReadAllText("Data/SeedData/genres.json");
      List<Genre> genres = System.Text.Json.JsonSerializer.Deserialize<List<Genre>>(genresJson);

      modelBuilder.Entity<Genre>().HasData(genres.ToArray());

      // Seed to Movies
      string moviesJson = System.IO.File.ReadAllText("Data/SeedData/movies.json");
      List<Movie> movies = System.Text.Json.JsonSerializer.Deserialize<List<Movie>>(moviesJson);
      modelBuilder.Entity<Movie>().HasData(movies.ToArray());

      // Seed to MovieGenres (nhiều-nhiều)
      string movieGenresJson = System.IO.File.ReadAllText("Data/SeedData/moviegenres.json");
      List<MovieGenre> movieGenres = System.Text.Json.JsonSerializer.Deserialize<List<MovieGenre>>(movieGenresJson);

      // Chắc chắn rằng MovieId và GenreId được match đúng trong bảng MovieGenre
      modelBuilder.Entity<MovieGenre>().HasData(movieGenres.ToArray());

      // Seed to Episodes
      string episodesJson = System.IO.File.ReadAllText("Data/SeedData/episodes.json");
      List<Episode> episodes = System.Text.Json.JsonSerializer.Deserialize<List<Episode>>(episodesJson);
      modelBuilder.Entity<Episode>().HasData(episodes.ToArray());

      // Seed to Servers
      string serversJson = System.IO.File.ReadAllText("Data/SeedData/servers.json");
      List<Server> servers = System.Text.Json.JsonSerializer.Deserialize<List<Server>>(serversJson);
      modelBuilder.Entity<Server>().HasData(servers.ToArray());


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
