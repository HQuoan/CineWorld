using AutoMapper;
using CineWorld.Services.ViewAPI.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using CineWorld.Services.ViewAPI;
using CineWorld.Services.ViewAPI.Attributes;
using CineWorld.Services.ViewAPI.Data;
using System.Threading.RateLimiting;
using CineWorld.Services.ViewAPI.Repositories.IRepositories;
using CineWorld.Services.ViewAPI.Repositories;
using CineWorld.Services.ViewAPI.Services.IService;
using CineWorld.Services.AuthAPI.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpLogging(options =>
{
  options.LoggingFields = HttpLoggingFields.RequestProperties | HttpLoggingFields.ResponsePropertiesAndHeaders;
});


// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(options =>
{
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddHttpContextAccessor();

builder.Services.AddRateLimiter(options =>
{
  options.AddPolicy<string>("IpRateLimit", httpContext =>
  {
    var ip = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                  ?? httpContext.Connection.RemoteIpAddress?.ToString();
    var movieId = httpContext.Request.Query["movieId"];
    var episodeId = httpContext.Request.Query["episodeId"];

    // Tạo partition key từ IP, MovieId và EpisodeId
    var partitionKey = $"{ip}-{movieId}-{episodeId}";

    return RateLimitPartition.GetFixedWindowLimiter(
        partitionKey,
        _ => new FixedWindowRateLimiterOptions
        {
          PermitLimit = 2,
          Window = TimeSpan.FromMinutes(5),
          QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
          QueueLimit = 0
        });
  });
});

builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
  option.SwaggerDoc("v1", new OpenApiInfo
  {
    Title = "View Management API",
    Version = "v1",
    Description = "",
    Contact = new OpenApiContact
    {
      Name = "Support Team",
      Email = "vuongvodtan@gmail.com",
      Url = new Uri("https://cineworld.io.vn/support")
    },
  });


  option.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
  {
    Name = "Authorization",
    Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
    In = ParameterLocation.Header,
    Type = SecuritySchemeType.ApiKey,
    Scheme = JwtBearerDefaults.AuthenticationScheme
  });
  option.AddSecurityRequirement(new OpenApiSecurityRequirement
  {
    {
      new OpenApiSecurityScheme
      {
        Reference = new OpenApiReference
        {
          Type = ReferenceType.SecurityScheme,
          Id = JwtBearerDefaults.AuthenticationScheme
        }
      }, new string[]{ }
    }
  });

  // Đường dẫn đến tệp XML
  var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
  var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
  if (File.Exists(xmlPath))
  {
    option.IncludeXmlComments(xmlPath);
  }
});

builder.AddAppAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IMovieService, MovieService>();

//builder.Services.AddHttpClient("User", u => u.BaseAddress = new Uri(builder.Configuration["ServiceUrls:AuthAPI"]));
builder.Services.AddHttpClient("Movie", u => u.BaseAddress = new Uri(builder.Configuration["ServiceUrls:MovieAPI"]));

// Thêm CORS
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowAllOrigins",
      policy =>
      {
        policy.WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
      });
});

// Configure Kestrel using Let's Encrypt certificate
builder.WebHost.ConfigureKestrel((context, options) =>
{
  if (context.HostingEnvironment.IsProduction())
  {
    options.ListenAnyIP(7002, listenOptions =>
    {
      listenOptions.UseHttps("/app/HttpsCerf/certificate.pfx", "yourpassword");
    });
  }
});

var app = builder.Build();

// Áp dụng CORS
app.UseCors("AllowAllOrigins");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
  c.SwaggerEndpoint("/swagger/v1/swagger.json", "Membership API v1");
});


app.UseHttpsRedirection();
app.UseRateLimiter();

// Đăng ký middleware Serilog để ghi log các request
//app.UseSerilogRequestLogging();

// Đăng ký middleware xử lý ngoại lệ toàn cục ngay sau logging
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpLogging();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

ApplyMigration();

app.Run();

void ApplyMigration()
{
  using (var scope = app.Services.CreateScope())
  {
    var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (_db.Database.GetPendingMigrations().Count() > 0)
    {
      _db.Database.Migrate();
    }
  }
}
