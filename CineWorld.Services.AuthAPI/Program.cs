using AutoMapper;
using CineWorld.EmailService;
using CineWorld.Services.AuthAPI;
using CineWorld.Services.AuthAPI.Attributes;
using CineWorld.Services.AuthAPI.Data;
using CineWorld.Services.AuthAPI.Extensions;
using CineWorld.Services.AuthAPI.Models;
using CineWorld.Services.AuthAPI.Services;
using CineWorld.Services.AuthAPI.Services.IService;
using CineWorld.Services.AuthAPI.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

// Configure Serilog (if needed)
// builder.Host.UseSerilog((context, services, configuration) => {
//   configuration.ReadFrom.Configuration(context.Configuration)
//   .ReadFrom.Services(services);
// });

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Configure AutoMapper
IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Configure JWT
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));
builder.Services.Configure<GoogleSettings>(builder.Configuration.GetSection("ApiSettings:Google"));


// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
  options.Password.RequireDigit = true;
  options.Password.RequiredLength = 8;
  options.Password.RequireNonAlphanumeric = true;
  options.Password.RequireUppercase = true;
  options.Password.RequireLowercase = true;
  options.Password.RequiredUniqueChars = 1;
  options.User.RequireUniqueEmail = true;
  options.SignIn.RequireConfirmedEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Add controllers
builder.Services.AddControllers();

// Add services
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(option =>
{
  option.SwaggerDoc("v1", new OpenApiInfo
  {
    Title = "Authentication and User Management API",
    Version = "v1",
    Description = "This API provides authentication, authorization, and user management functionalities. It includes features like user registration, login, role assignment, password management, and user profile operations.",
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

  var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
  var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
  if (File.Exists(xmlPath))
  {
    option.IncludeXmlComments(xmlPath);
  }
});

// Add Authentication
builder.AddAppAuthentication();

// Add Authorization Policies
//builder.Services.AddAuthorization(options =>
//{
//  options.AddPolicy("GoogleAuth", policy =>
//  {
//    policy.AuthenticationSchemes.Add(GoogleDefaults.AuthenticationScheme);
//    policy.RequireAuthenticatedUser();
//  });
//});

// Register other services
builder.Services.AddScoped<IMembershipService, MembershipService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddHttpClient("Membership", u => u.BaseAddress = new Uri(builder.Configuration["ServiceUrls:MembershipAPI"]));

// Add CORS
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowAllOrigins",
      policy =>
      {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5174")
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
    options.ListenAnyIP(7000, listenOptions =>
    {
      listenOptions.UseHttps("/app/HttpsCerf/certificate.pfx", "yourpassword");
    });
  }
});


var app = builder.Build();

// Apply CORS Policy
app.UseCors("AllowAllOrigins");

// Apply migrations
ApplyMigration();

// Seed roles
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


// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
  c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth API v1");
});

app.UseHttpsRedirection();

// Register global exception middleware
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
