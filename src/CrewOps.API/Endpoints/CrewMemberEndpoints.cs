using Microsoft.EntityFrameworkCore;
using CrewOps.API.Data;
using CrewOps.API.Models;

namespace CrewOps.API.Endpoints;

public static class CrewMemberEndpoints
{
    public static RouteGroupBuilder MapCrewMemberEndpoints(this RouteGroupBuilder group)
    {
        // GET all crew members
        group.MapGet("/", async (CrewOpsDbContext db) =>
        {
            return Results.Ok(await db.CrewMembers.ToListAsync());
        })
        .WithName("GetAllCrewMembers")
        .WithOpenApi();

        // GET a single crew member by ID
        group.MapGet("/{id}", async (int id, CrewOpsDbContext db) =>
        {
            var member = await db.CrewMembers.FindAsync(id);
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
        group.MapPut("/{id}", async (int id, CrewMember updatedMember, CrewOpsDbContext db) =>
        {
            var member = await db.CrewMembers.FindAsync(id);
            if (member is null) return Results.NotFound();

            member.FirstName = updatedMember.FirstName;
            member.LastName = updatedMember.LastName;
            member.Email = updatedMember.Email;
            member.Status = updatedMember.Status;

            await db.SaveChangesAsync();
            return Results.Ok(member);
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
