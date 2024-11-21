using AutoMapper;
using CineWorld.EmailService;
using CineWorld.Services.AuthAPI;
using CineWorld.Services.AuthAPI.Attributes;
using CineWorld.Services.AuthAPI.Data;
using CineWorld.Services.AuthAPI.Extensions;
using CineWorld.Services.AuthAPI.Models;
using CineWorld.Services.AuthAPI.Services;
using CineWorld.Services.AuthAPI.Services.IService;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
//Serilog
//builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) => {

//  loggerConfiguration
//  .ReadFrom.Configuration(context.Configuration) //read configuration settings from built-in IConfiguration
//  .ReadFrom.Services(services); //read out current app's services and make them available to serilog
//});

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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
  options.SignIn.RequireConfirmedEmail = true; // Bắt buộc xác nhận email
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllers();

builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(option =>
{
  option.SwaggerDoc("v1", new OpenApiInfo
  {
    Title = "Car Management API",
    Version = "v1",
    Description = "API for managing cars, including features to list, add, and delete cars."
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
  option.IncludeXmlComments(xmlPath);
});

builder.AddAppAuthentication();
builder.Services.AddAuthorization(options =>
{
  options.AddPolicy("GoogleAuth", policy =>
  {
    policy.AuthenticationSchemes.Add(GoogleDefaults.AuthenticationScheme);
    policy.RequireAuthenticatedUser();
  });
});

builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IMembershipService, MembershipService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddHttpClient("Membership", u => u.BaseAddress = new Uri(builder.Configuration["ServiceUrls:MembershipAPI"]));

// Thêm CORS
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowAllOrigins",
      policy =>
      {
        policy.WithOrigins("http://localhost:5173", "https://localhost:7000")
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials();
      });
});

var app = builder.Build();

// Áp dụng CORS
app.UseCors("AllowAllOrigins");

ApplyMigration();

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
app.UseSwagger();
app.UseSwaggerUI(c =>
{
  c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth API v1");
});



app.UseHttpsRedirection();

// Đăng ký middleware Serilog để ghi log các request
//app.UseSerilogRequestLogging();

// Đăng ký middleware xử lý ngoại lệ toàn cục ngay sau logging
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();


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