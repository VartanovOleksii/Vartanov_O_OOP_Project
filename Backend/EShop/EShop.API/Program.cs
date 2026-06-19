using System.Text;
using EShop.API.Data;
using EShop.Domain.DTOs;
using EShop.Domain.Interfaces;
using EShop.Domain.Models;
using EShop.Domain.Ports;
using EShop.Domain.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

const string FrontendCorsPolicy = "FrontendCorsPolicy";

// Allow the SvelteKit dev server (D:\KhAI\CourseWork\Frontend) to call the API.
// JWT travels in the Authorization header, so AllowCredentials() is not needed.
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var connString = builder.Configuration.GetConnectionString("Default");

// EF Core + SQLite
builder.Services.AddDbContext<AppDbContext>(opts => opts.UseSqlite(connString));

// ASP.NET Core Identity — built-in password rules disabled; CustomPasswordValidator handles them
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(opts =>
    {
        opts.Password.RequireDigit = false;
        opts.Password.RequireLowercase = false;
        opts.Password.RequireUppercase = false;
        opts.Password.RequireNonAlphanumeric = false;
        opts.Password.RequiredLength = 1;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddPasswordValidator<CustomPasswordValidator>();

// Domain services
builder.Services.AddScoped<Guest>();
builder.Services.AddScoped<IJsonDataPort<ProductExportDto>, JsonDataPort<ProductExportDto>>();
builder.Services.AddScoped<IJsonDataPort<OrderExportDto>, JsonDataPort<OrderExportDto>>();

// JWT authentication
var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services
    .AddAuthentication(options =>
    {
        // Override the cookie-based defaults that AddIdentity registers so that
        // API requests are authenticated and challenged via JWT Bearer.
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(opts =>
    {
        opts.MapInboundClaims = false;
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            NameClaimType = "sub",
            RoleClaimType = "role"
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Apply migrations and seed roles on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var roleName in new[] { "Buyer", "Seller" })
    {
        if (!await roleManager.RoleExistsAsync(roleName))
            await roleManager.CreateAsync(new IdentityRole(roleName));
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors(FrontendCorsPolicy);

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
