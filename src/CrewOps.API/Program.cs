using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using CrewOps.API.Data;
using CrewOps.API.Models;
using CrewOps.API.Endpoints;
using CrewOps.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure JSON to serialize enums as strings (e.g., "Pending" instead of 0)
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

// Register the DbContext with dependency injection
// This tells the app: "When someone asks for CrewOpsDbContext, create one using SQLite"
builder.Services.AddDbContext<CrewOpsDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register JwtService
builder.Services.AddScoped<JwtService>();

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "CrewOps",
            ValidAudience = "CrewOps",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(JwtService.SecretKeyValue))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Authentication & Authorization middleware (order matters!)
app.UseAuthentication();
app.UseAuthorization();

// ============================================
// YOUR FIRST CUSTOM ENDPOINT
// ============================================

// Simple GET endpoint - returns plain text
app.MapGet("/", () => "Welcome to CrewOps API!");

// GET endpoint with a route parameter
app.MapGet("/hello/{name}", (string name) => $"Hello, {name}! Welcome to CrewOps.");

// GET endpoint returning an object (automatically converted to JSON)
app.MapGet("/api/status", () => new
{
    Status = "Healthy",
    Application = "CrewOps API",
    Version = "1.0.0",
    Timestamp = DateTime.UtcNow
});

// ============================================
// API ENDPOINT GROUPS
// ============================================

app.MapGroup("/api/crewmembers")
    .WithTags("CrewMembers")
    .MapCrewMemberEndpoints();

app.MapGroup("/api/jobs")
    .WithTags("Jobs")
    .MapJobEndpoints();

app.MapGroup("/api/time")
    .WithTags("TimeTracking")
    .MapTimeEntryEndpoints();

app.MapGroup("/api/auth")
    .WithTags("Authentication")
    .MapAuthEndpoints();

// ============================================
// DEFAULT WEATHER ENDPOINT (from template)
// ============================================

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
