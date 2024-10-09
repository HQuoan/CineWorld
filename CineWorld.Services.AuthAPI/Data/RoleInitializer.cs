using CineWorld.Services.AuthAPI.Models.Dto;
using CineWorld.Services.AuthAPI.Models;
using CineWorld.Services.AuthAPI.Utilities;
using Microsoft.AspNetCore.Identity;

namespace CineWorld.Services.AuthAPI.Data
{
  public static class RoleInitializer
  {
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
      var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
      var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

      string[] roles = { SD.RoleCustomer, SD.RoleAdmin};

      foreach (var role in roles)
      {
        // Kiểm tra xem role đã tồn tại chưa
        if (!await roleManager.RoleExistsAsync(role))
        {
          // Tạo role nếu chưa tồn tại
          await roleManager.CreateAsync(new IdentityRole(role));
        }
      }

      // Create default admin user
      var adminEmail = "admin@gmail.com";
     
     
      var adminUserFromDb = await userManager.FindByEmailAsync(adminEmail);
      if (adminUserFromDb == null)
      {
        ApplicationUser adminUser = new()
        {
          UserName = adminEmail,
          Email = adminEmail,
          NormalizedEmail = adminEmail.ToUpper(),
          Name = "Admin",
          Gender = "Male",
          DateOfBirth = DateTime.UtcNow,
          MembershipEndDate = DateTime.UtcNow.AddYears(100)
        };

        var result = await userManager.CreateAsync(adminUser, "Admin@123");
        if (result.Succeeded)
        {
          await userManager.AddToRoleAsync(adminUser, SD.RoleAdmin);
        }
        else
        {
          throw new Exception("Failed to create admin user.");
        }
      }
    }
  }
}
