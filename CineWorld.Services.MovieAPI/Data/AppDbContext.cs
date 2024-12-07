using CineWorld.Services.MovieAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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

      //// Seed to Movies
      //string moviesJson = System.IO.File.ReadAllText("Data/SeedData/movies.json");
      //List<Movie> movies = System.Text.Json.JsonSerializer.Deserialize<List<Movie>>(moviesJson);
      //modelBuilder.Entity<Movie>().HasData(movies.ToArray());

      //// Seed to MovieGenres (nhiều-nhiều)
      //string movieGenresJson = System.IO.File.ReadAllText("Data/SeedData/moviegenres.json");
      //List<MovieGenre> movieGenres = System.Text.Json.JsonSerializer.Deserialize<List<MovieGenre>>(movieGenresJson);

      //// Chắc chắn rằng MovieId và GenreId được match đúng trong bảng MovieGenre
      //modelBuilder.Entity<MovieGenre>().HasData(movieGenres.ToArray());

      //// Seed to Episodes
      //string episodesJson = System.IO.File.ReadAllText("Data/SeedData/episodes.json");
      //List<Episode> episodes = System.Text.Json.JsonSerializer.Deserialize<List<Episode>>(episodesJson);
      //modelBuilder.Entity<Episode>().HasData(episodes.ToArray());

      //// Seed to Servers
      //string serversJson = System.IO.File.ReadAllText("Data/SeedData/servers.json");
      //List<Server> servers = System.Text.Json.JsonSerializer.Deserialize<List<Server>>(serversJson);
      //modelBuilder.Entity<Server>().HasData(servers.ToArray());


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

      // Tạo chỉ mục (index) cho các trường trong bảng Movie
      modelBuilder.Entity<Movie>()
          .HasIndex(m => m.Name)
          .HasName("IX_Movie_Name");

      modelBuilder.Entity<Movie>()
          .HasIndex(m => m.Slug)
          .HasName("IX_Movie_Slug");

      modelBuilder.Entity<Movie>()
          .HasIndex(m => m.CategoryId)
          .HasName("IX_Movie_CategoryId");

      modelBuilder.Entity<Movie>()
          .HasIndex(m => m.CountryId)
          .HasName("IX_Movie_CountryId");

      modelBuilder.Entity<Movie>()
          .HasIndex(m => m.Year)
          .HasName("IX_Movie_Year");

      modelBuilder.Entity<Movie>()
          .HasIndex(m => m.View)
          .HasName("IX_Movie_View");

      modelBuilder.Entity<Movie>()
          .HasIndex(m => m.IsHot)
          .HasName("IX_Movie_IsHot");

      modelBuilder.Entity<Movie>()
          .HasIndex(m => m.CreatedDate)
          .HasName("IX_Movie_CreatedDate");

      modelBuilder.Entity<Movie>()
          .HasIndex(m => m.UpdatedDate)
          .HasName("IX_Movie_UpdatedDate");

    }

    public async Task SeedDataAsync()
    {
      await SeedEntityAsync<Movie>("Data/SeedData/movies.json", Movies);
      await SeedEntityAsync<MovieGenre>("Data/SeedData/moviegenres.json", MovieGenres);
      await SeedEntityAsync<Episode>("Data/SeedData/episodes.json", Episodes);
      await SeedEntityAsync<Server>("Data/SeedData/servers.json", Servers);

    }

    private async Task SeedEntityAsync<TEntity>(string filePath, DbSet<TEntity> dbSet) where TEntity : class
    {
      if (File.Exists(filePath))
      {
        string json = await File.ReadAllTextAsync(filePath);
        List<TEntity> entities = JsonSerializer.Deserialize<List<TEntity>>(json);
        if (entities != null && entities.Count > 0)
        {
          // Tạo dictionary để ánh xạ kiểu thực thể với tên bảng
          var identityInsertTables = new Dictionary<Type, string>
            {
                { typeof(Movie), "[dbo].[Movies]" },
                { typeof(MovieGenre), "[dbo].[MovieGenres]" },
                { typeof(Episode), "[dbo].[Episodes]" },
                { typeof(Server), "[dbo].[Servers]" }
            };

          // Bắt đầu transaction để chèn dữ liệu
          using (var transaction = await Database.BeginTransactionAsync())
          {
            try
            {
              // Kiểm tra xem kiểu thực thể có trong dictionary không
              if (identityInsertTables.TryGetValue(typeof(TEntity), out var tableName))
              {
                await Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT {tableName} ON");
              }

              await dbSet.AddRangeAsync(entities);
              await SaveChangesAsync();

              if (identityInsertTables.TryGetValue(typeof(TEntity), out var tableToTurnOff))
              {
                await Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT {tableToTurnOff} OFF");
              }

              await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
              await transaction.RollbackAsync();
              throw new Exception($"Error seeding data: {ex.Message}", ex);
            }
          }
        }
      }
    }



  }
}
