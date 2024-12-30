using AutoMapper;
using CineWorld.Services.MovieAPI;
using CineWorld.Services.MovieAPI.Attributes;
using CineWorld.Services.MovieAPI.Data;
using CineWorld.Services.MovieAPI.Extensions;
using CineWorld.Services.MovieAPI.Repositories;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;
using CineWorld.Services.MovieAPI.Services;
using CineWorld.Services.MovieAPI.Services.IService;
using CineWorld.Services.MovieAPI.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
//Serilog
//builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) => {

//  loggerConfiguration
//  .ReadFrom.Configuration(context.Configuration) //read configuration settings from built-in IConfiguration
//  .ReadFrom.Services(services); //read out current app's services and make them available to serilog
//});

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
//builder.Services.AddRateLimiter(options =>
//{
//  options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
//    RateLimitPartition.GetFixedWindowLimiter(
//        partitionKey: context.Connection.RemoteIpAddress?.ToString(), // Phân vùng theo IP
//        factory: _ => new FixedWindowRateLimiterOptions
//        {
//          PermitLimit = 100,
//          Window = TimeSpan.FromMinutes(1),
//          QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
//          QueueLimit = 0
//        }
//    )
//);


//  options.RejectionStatusCode = 429; // HTTP Status 429 - Too Many Requests
//});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
  option.SwaggerDoc("v1", new OpenApiInfo
  {
    Title = "Movie Management API",
    Version = "v1",
    Description = "This API provides functionalities for managing movies, series, and related media. It includes features like adding, updating, deleting, and retrieving movie and series details, managing genres, categories, and user preferences.",
    Contact = new OpenApiContact
    {
      Name = "Support Team",
      Email = "vuongvodtan@gmail.com",
      Url = new Uri("https://cineworld.io.vn")
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
builder.Services.AddScoped<IUtil, Util>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMembershipService, MembershipService>();

builder.Services.AddScoped<BackendApiAuthenticationHttpClientHandler>();
builder.Services.AddHttpClient("User", u => u.BaseAddress = new Uri(builder.Configuration["ServiceUrls:AuthAPI"])).AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>();
builder.Services.AddHttpClient("Membership", u => u.BaseAddress = new Uri(builder.Configuration["ServiceUrls:MembershipAPI"]));

// Thêm CORS
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowAllOrigins",
        policy =>
        {
          policy.SetIsOriginAllowed(_ => true) // Cho phép tất cả các origin
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
    options.ListenAnyIP(7001, listenOptions =>
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
  c.SwaggerEndpoint("/swagger/v1/swagger.json", "Movie API v1");
});

app.UseHttpsRedirection();

//app.UseRateLimiter();

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

    // Kiểm tra xem đã có dữ liệu trong bảng hay chưa
    if (!_db.Movies.Any()) // Kiểm tra bảng Movies hoặc bảng phù hợp
    {
      _db.SeedDataAsync().Wait();
    }
  }
}
