using AutoMapper;
using CineWorld.Services.EpisodeAPI;
using CineWorld.Services.EpisodeAPI.Attributes;
using CineWorld.Services.EpisodeAPI.Data;
using CineWorld.Services.EpisodeAPI.Extensions;
using CineWorld.Services.EpisodeAPI.Repositories;
using CineWorld.Services.EpisodeAPI.Repositories.IRepositories;
using CineWorld.Services.EpisodeAPI.Services;
using CineWorld.Services.EpisodeAPI.Services.IService;
using CineWorld.Services.EpisodeAPI.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
//Serilog
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) => {

  loggerConfiguration
  .ReadFrom.Configuration(context.Configuration) //read configuration settings from built-in IConfiguration
  .ReadFrom.Services(services); //read out current app's services and make them available to serilog
});

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
builder.Services.AddScoped<BackendApiAuthenticationHttpClientHandler>();

builder.Services.AddScoped<IMovieService, MovieService>();

builder.Services.AddHttpClient("Movie", u => u.BaseAddress = new Uri(builder.Configuration["ServiceUrls:MovieAPI"])).AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>();

builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
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
});

builder.AddAppAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUtil, Util>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Đăng ký middleware Serilog để ghi log các request
app.UseSerilogRequestLogging();

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
