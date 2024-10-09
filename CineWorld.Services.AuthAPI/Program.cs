using CineWorld.MessageBus;
using CineWorld.Services.AuthAPI.Attributes;
using CineWorld.Services.AuthAPI.Data;
using CineWorld.Services.AuthAPI.Models;
using CineWorld.Services.AuthAPI.Services;
using CineWorld.Services.AuthAPI.Services.IService;
using CommandLine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
//Serilog
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) => {

  loggerConfiguration
  .ReadFrom.Configuration(context.Configuration) //read configuration settings from built-in IConfiguration
  .ReadFrom.Services(services); //read out current app's services and make them available to serilog
});

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
  // Các tùy chỉnh về mật khẩu
  options.Password.RequireDigit = true; 
  options.Password.RequiredLength = 8; 
  options.Password.RequireNonAlphanumeric = true; 
  options.Password.RequireUppercase = true; 
  options.Password.RequireLowercase = true;
  options.Password.RequiredUniqueChars = 1; 
  options.User.RequireUniqueEmail = true; // Yêu cầu email duy nhất cho mỗi người dùng
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllers();

builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMessageBus, MessageBus>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Seed các role
using (var scope = app.Services.CreateScope())
{
  var services = scope.ServiceProvider;
  try
  {
    await RoleInitializer.InitializeAsync(services);
  }
  catch (Exception ex)
  {
    throw new Exception("An error occurred while seeding the roles.", ex);
  }
}


// Configure the HTTP request pipeline.
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