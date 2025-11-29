using Microsoft.EntityFrameworkCore;
using CrewOps.API.Data;
using CrewOps.API.Models;
using CrewOps.API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the DbContext with dependency injection
// This tells the app: "When someone asks for CrewOpsDbContext, create one using SQLite"
builder.Services.AddDbContext<CrewOpsDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

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
