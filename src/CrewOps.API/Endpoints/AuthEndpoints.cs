using Microsoft.EntityFrameworkCore;
using CrewOps.API.Data;
using CrewOps.API.Services;

namespace CrewOps.API.Endpoints;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this RouteGroupBuilder group)
    {
        // POST /api/auth/login
        group.MapPost("/login", async (LoginRequest request, CrewOpsDbContext db, JwtService jwtService) =>
        {
            // Find user by email
            var member = await db.CrewMembers
                .FirstOrDefaultAsync(c => c.Email == request.Email);

            if (member is null)
                return Results.Unauthorized();

            // Verify password (simple string comparison for now - will add hashing later)
            if (member.PasswordHash != request.Password)
                return Results.Unauthorized();

            // Generate JWT token
            var token = jwtService.GenerateToken(member);

            return Results.Ok(new LoginResponse(
                token,
                member.Id,
                $"{member.FirstName} {member.LastName}",
                member.Role
            ));
        })
        .WithName("Login")
        .WithOpenApi()
        .AllowAnonymous();

        // POST /api/auth/set-password (temporary endpoint for setting passwords)
        group.MapPost("/set-password", async (SetPasswordRequest request, CrewOpsDbContext db) =>
        {
            var member = await db.CrewMembers.FindAsync(request.CrewMemberId);
            if (member is null)
                return Results.NotFound("Crew member not found");

            // Set password (plain text for now - will add hashing later)
            member.PasswordHash = request.Password;
            await db.SaveChangesAsync();

            return Results.Ok(new { Message = "Password set successfully" });
        })
        .WithName("SetPassword")
        .WithOpenApi()
        .AllowAnonymous(); // In production, this should require admin auth

        return group;
    }
}

// Request/Response DTOs
public record LoginRequest(string Email, string Password);
public record LoginResponse(string Token, int UserId, string Name, string Role);
public record SetPasswordRequest(int CrewMemberId, string Password);
