using CineWorld.Services.MembershipAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CineWorld.Services.MembershipAPI.Data
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    public DbSet<Package> Packages { get; set; }
    public DbSet<Coupon> Coupons { get; set; }
    public DbSet<Receipt> Receipts { get; set; }
    public DbSet<MemberShip> MemberShips { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Coupon>()
       .Property(c => c.DiscountAmount)
       .HasPrecision(18, 2); 

      modelBuilder.Entity<Package>()
          .Property(p => p.Price)
          .HasPrecision(18, 2);

      modelBuilder.Entity<Receipt>()
          .Property(r => r.DiscountAmount)
          .HasPrecision(18, 2);

      modelBuilder.Entity<Receipt>()
          .Property(r => r.PackagePrice)
          .HasPrecision(18, 2);

      // Seed to Packages 
      string packagesJson = System.IO.File.ReadAllText("Data/SeedData/packages.json");
      List<Package> packages = System.Text.Json.JsonSerializer.Deserialize<List<Package>>(packagesJson);
      modelBuilder.Entity<Package>().HasData(packages.ToArray());



      // mặc định tạo CreatedDate khi tạo và không update được 
      modelBuilder.Entity<Package>()
           .Property(m => m.CreatedDate)
           .HasDefaultValueSql("GETDATE()")
           .ValueGeneratedOnAdd()
           .Metadata.SetAfterSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore);
      modelBuilder.Entity<Receipt>()
           .Property(m => m.CreatedDate)
           .HasDefaultValueSql("GETDATE()")
           .ValueGeneratedOnAdd()
           .Metadata.SetAfterSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore);
      modelBuilder.Entity<Coupon>()
          .Property(m => m.CreatedDate)
          .HasDefaultValueSql("GETDATE()")
          .ValueGeneratedOnAdd()
          .Metadata.SetAfterSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore);


      // Đánh index coupon 
      modelBuilder.Entity<Coupon>()
       .HasIndex(c => c.CouponCode)
       .IsUnique();
      modelBuilder.Entity<Coupon>()
       .HasIndex(c => c.IsActive);
      modelBuilder.Entity<Coupon>()
          .HasIndex(c => c.CreatedDate);

      // Đánh index membership 
      modelBuilder.Entity<MemberShip>()
       .HasIndex(m => m.UserId);
      modelBuilder.Entity<MemberShip>()
          .HasIndex(m => m.ExpirationDate);
      modelBuilder.Entity<MemberShip>()
          .HasIndex(m => m.RenewalStartDate);
      modelBuilder.Entity<MemberShip>()
          .HasIndex(m => m.UserEmail);

      // Đánh index receipt 
      modelBuilder.Entity<Receipt>()
          .HasIndex(r => r.UserId);
      modelBuilder.Entity<Receipt>()
          .HasIndex(r => r.Email);
      modelBuilder.Entity<Receipt>()
          .HasIndex(r => r.PackageId);
      modelBuilder.Entity<Receipt>()
          .HasIndex(r => r.CreatedDate);
      modelBuilder.Entity<Receipt>()
          .HasIndex(r => r.Status);
      modelBuilder.Entity<Receipt>()
          .HasIndex(r => r.CouponCode);
    }

  }
}
