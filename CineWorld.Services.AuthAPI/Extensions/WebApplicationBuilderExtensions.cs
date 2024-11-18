using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace CineWorld.Services.AuthAPI.Extensions
{
  public static class WebApplicationBuilderExtensions
  {
    public static WebApplicationBuilder AddAppAuthentication(this WebApplicationBuilder builder)
    {
      var secret = builder.Configuration.GetValue<string>("ApiSettings:JwtOptions:Secret");
      var issuer = builder.Configuration.GetValue<string>("ApiSettings:JwtOptions:Issuer");
      var audience = builder.Configuration.GetValue<string>("ApiSettings:JwtOptions:Audience");
      var googleClientId = builder.Configuration.GetValue<string>("ApiSettings:Google:ClientId");
      var googleClientSecret = builder.Configuration.GetValue<string>("ApiSettings:Google:ClientSecret");

      var key = Encoding.ASCII.GetBytes(secret);

      builder.Services.AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer(x =>
      {
        x.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(key),
          ValidateIssuer = true,
          ValidIssuer = issuer,
          ValidateAudience = true,
          ValidAudience = audience,
        };
      })
      .AddGoogle(googleOptions =>
      {
        googleOptions.ClientId = googleClientId;
        googleOptions.ClientSecret = googleClientSecret;
        googleOptions.CallbackPath = "/signin-google"; // Phải khớp với cấu hình của Google Console
      })
      .AddCookie();
      
      return builder;
    }
  }
}
