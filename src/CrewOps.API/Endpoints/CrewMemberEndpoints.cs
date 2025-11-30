using Microsoft.EntityFrameworkCore;
using CrewOps.API.Data;
using CrewOps.API.Models;

namespace CrewOps.API.Endpoints;

public static class CrewMemberEndpoints
{
    public static RouteGroupBuilder MapCrewMemberEndpoints(this RouteGroupBuilder group)
    {
        // GET all crew members (excludes passwordHash for security)
        group.MapGet("/", async (CrewOpsDbContext db) =>
        {
            var members = await db.CrewMembers
                .Select(m => new
                {
                    m.Id,
                    m.FirstName,
                    m.LastName,
                    m.Email,
                    m.Status,
                    m.Role,
                    m.CreatedAt
                })
                .ToListAsync();
            return Results.Ok(members);
        })
        .WithName("GetAllCrewMembers")
        .WithOpenApi();

        // GET a single crew member by ID (excludes passwordHash for security)
        group.MapGet("/{id}", async (int id, CrewOpsDbContext db) =>
        {
            var member = await db.CrewMembers
                .Where(m => m.Id == id)
                .Select(m => new
                {
                    m.Id,
                    m.FirstName,
                    m.LastName,
                    m.Email,
                    m.Status,
                    m.Role,
                    m.CreatedAt
                })
                .FirstOrDefaultAsync();
            return member is not null ? Results.Ok(member) : Results.NotFound();
        })
        .WithName("GetCrewMember")
        .WithOpenApi();

        // POST - Create a new crew member
        group.MapPost("/", async (CreateCrewMemberRequest request, CrewOpsDbContext db) =>
        {
            // Hash the password using BCrypt
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var member = new CrewMember
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Status = "Active",
                Role = request.Role ?? "CrewMember",
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            db.CrewMembers.Add(member);
            await db.SaveChangesAsync();

            // Return response without password/hash
            return Results.Created($"/api/crewmembers/{member.Id}", new
            {
                member.Id,
                member.FirstName,
                member.LastName,
                member.Email,
                member.Status,
                member.Role,
                member.CreatedAt
            });
        })
        .WithName("CreateCrewMember")
        .WithOpenApi();

        // PUT - Update an existing crew member
        group.MapPut("/{id}", async (int id, UpdateCrewMemberRequest request, CrewOpsDbContext db) =>
        {
            var member = await db.CrewMembers.FindAsync(id);
            if (member is null) return Results.NotFound();

            member.FirstName = request.FirstName;
            member.LastName = request.LastName;
            member.Email = request.Email;
            member.Status = request.Status;

            await db.SaveChangesAsync();

            // Return response without password/hash
            return Results.Ok(new
            {
                member.Id,
                member.FirstName,
                member.LastName,
                member.Email,
                member.Status,
                member.Role,
                member.CreatedAt
            });
        })
        .WithName("UpdateCrewMember")
        .WithOpenApi();

        // DELETE - Remove a crew member
        group.MapDelete("/{id}", async (int id, CrewOpsDbContext db) =>
        {
            var member = await db.CrewMembers.FindAsync(id);
            if (member is null) return Results.NotFound();

            db.CrewMembers.Remove(member);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithName("DeleteCrewMember")
        .WithOpenApi();
        

        return group;
    }
}

// Request DTO for creating crew members
public record CreateCrewMemberRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string? Role
);

// Request DTO for updating crew members (no password - use set-password endpoint)
public record UpdateCrewMemberRequest(
    string FirstName,
    string LastName,
    string Email,
    string Status
);
