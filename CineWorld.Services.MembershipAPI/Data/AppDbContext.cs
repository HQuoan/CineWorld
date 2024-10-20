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
          .HasIndex(c => c.CouponCode)
          .IsUnique();

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
    }

  }
}
