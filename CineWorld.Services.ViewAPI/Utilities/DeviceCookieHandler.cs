namespace CineWorld.Services.ViewAPI.Utilities
{
  public class DeviceCookieHandler
  {
    private const string CookieName = "DeviceId";
    private const string ContextItemKey = "DeviceId";

    public string Set(HttpContext httpContext)
    {
      var deviceId = Guid.NewGuid().ToString();

      // Lưu vào Response.Cookies
      httpContext.Response.Cookies.Append(CookieName, deviceId, new CookieOptions
      {
        HttpOnly = true,
        Secure = true,
        Expires = DateTimeOffset.UtcNow.AddYears(1)
      });

      // Lưu tạm vào HttpContext.Items
      httpContext.Items[ContextItemKey] = deviceId;

      return deviceId;
    }

    public string Get(HttpContext httpContext)
    {
      // Kiểm tra trong HttpContext.Items trước
      if (httpContext.Items.TryGetValue(ContextItemKey, out var itemValue) && itemValue is string deviceId)
      {
        return deviceId;
      }

      // Nếu không có, lấy từ Request.Cookies
      httpContext.Request.Cookies.TryGetValue(CookieName, out deviceId);
      return deviceId;
    }
  }
}
