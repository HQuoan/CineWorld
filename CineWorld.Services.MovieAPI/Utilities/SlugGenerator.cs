using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Slugify;
using System.Text.RegularExpressions;

namespace CineWorld.Services.MovieAPI.Utilities
{
  public static class SlugGenerator
  {
    public static string GenerateSlug(string name)
    {
      string slug = name.ToLowerInvariant();
      SlugHelper helper = new SlugHelper();

      return helper.GenerateSlug(slug);
    }

    public static string CreateUniqueSlugAsync(string name)
    {
      // Tạo slug cơ bản từ tên
      string baseSlug = SlugGenerator.GenerateSlug(name);
      // Lấy số giây tính từ epoch
      long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
      string uniqueSlug = $"{baseSlug}-{timestamp}";

      return uniqueSlug;
    }

  }
}
