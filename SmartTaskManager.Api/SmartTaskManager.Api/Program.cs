using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartTaskManager.Api.Data;
using SmartTaskManager.Api.Helpers;
using SmartTaskManager.Api.Repositories;
using SmartTaskManager.Api.Repositories.Interfaces;
using SmartTaskManager.Api.Services;
using SmartTaskManager.Api.Services.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =====================================
// 1. Add services to the container
// =====================================

// Enable Controllers (API endpoints)
builder.Services.AddControllers();

// =====================================
// Swagger (Human-friendly UI)
// =====================================
// Used for:
// - Testing APIs in browser
// - Debugging endpoints
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =====================================
// OpenAPI (Machine-readable JSON)
// =====================================
// Used for:
// - API clients (frontend/mobile)
// - Code generation tools
builder.Services.AddOpenApi();

// =====================================
// Database (EF Core + SQL Server)
// =====================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// =====================================
// Dependency Injection (Repositories)
// =====================================
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

// =====================================
// Helpers
// =====================================
builder.Services.AddScoped<JwtHelper>();

// =====================================
// Services (Business Logic)
// =====================================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITaskService, TaskService>();

// =====================================
// Authentication (JWT)
// =====================================
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// =====================================
// Authorization
// =====================================
builder.Services.AddAuthorization();

var app = builder.Build();

// =====================================
// 2. Configure HTTP request pipeline
// =====================================

// Redirect HTTP → HTTPS
app.UseHttpsRedirection();

// =====================================
// Authentication & Authorization
// =====================================
// MUST be in this order:
// 1. Authentication → validates token
// 2. Authorization → checks permissions
app.UseAuthentication();
app.UseAuthorization();

// =====================================
// Development Tools
// =====================================
if (app.Environment.IsDevelopment())
{
    // Swagger UI (browser testing)
    // URL: https://localhost:xxxx/swagger
    app.UseSwagger();
    app.UseSwaggerUI();

    // OpenAPI JSON endpoint
    // URL: https://localhost:xxxx/openapi/v1.json
    app.MapOpenApi();
}

// =====================================
// Map Controllers
// =====================================
app.MapControllers();

// =====================================
// Run Application
// =====================================
app.Run();