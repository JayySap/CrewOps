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
        group.MapPost("/", async (CrewMember member, CrewOpsDbContext db) =>
        {
            member.CreatedAt = DateTime.UtcNow;
            db.CrewMembers.Add(member);
            await db.SaveChangesAsync();
            return Results.Created($"/api/crewmembers/{member.Id}", member);
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
