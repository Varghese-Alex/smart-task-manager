using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------
// Add services to the container
// -----------------------------

// Enables MVC Controllers
builder.Services.AddControllers();

// -----------------------------
// Swagger (Human-friendly UI)
// -----------------------------
// Used for:
// - Testing APIs in browser
// - Debugging endpoints
// - Development & demos
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// -----------------------------
// OpenAPI (Machine-readable JSON)
// -----------------------------
// Used for:
// - API clients (frontend/mobile)
// - Code generation tools
// - External integrations
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// -----------------------------
// Configure HTTP request pipeline
// -----------------------------
if (app.Environment.IsDevelopment())
{
    // -----------------------------
    // Swagger UI (visual interface)
    // URL: https://localhost:xxxx/swagger
    // -----------------------------
    app.UseSwagger();
    app.UseSwaggerUI();

    // -----------------------------
    // OpenAPI JSON endpoint
    // URL: https://localhost:xxxx/openapi/v1.json
    // -----------------------------
    app.MapOpenApi();
}

// Redirect HTTP → HTTPS
app.UseHttpsRedirection();

// Authorization middleware (will be useful later)
app.UseAuthorization();

// Maps controller routes
app.MapControllers();

app.Run();