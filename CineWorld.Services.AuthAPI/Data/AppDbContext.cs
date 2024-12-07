using CineWorld.Services.AuthAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CineWorld.Services.AuthAPI.Data
{
  public class AppDbContext : IdentityDbContext<ApplicationUser>
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<ApplicationUser> ApplicationUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<ApplicationUser>()
        .HasIndex(u => u.Email)
        .IsUnique(); 

      modelBuilder.Entity<ApplicationUser>()
          .HasIndex(u => u.FullName);

      modelBuilder.Entity<ApplicationUser>()
          .HasIndex(u => u.Gender);

      modelBuilder.Entity<ApplicationUser>()
          .HasIndex(u => u.DateOfBirth);

      modelBuilder.Entity<IdentityUserRole<string>>()
          .HasIndex(ur => new { ur.UserId, ur.RoleId });

      modelBuilder.Entity<IdentityRole>()
          .HasIndex(r => r.Name);

    }
  }
}
