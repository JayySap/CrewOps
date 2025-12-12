using Microsoft.EntityFrameworkCore;
using CrewOps.API.Data;
using CrewOps.Shared.Models;

namespace CrewOps.API.Endpoints;

public static class JobEndpoints
{
    public static RouteGroupBuilder MapJobEndpoints(this RouteGroupBuilder group)
    {
        // GET all jobs
        group.MapGet("/", async (CrewOpsDbContext db) =>
        {
            return Results.Ok(await db.Jobs.ToListAsync());
        })
        .WithName("GetAllJobs")
        .WithOpenApi();

        // GET a single job by ID
        group.MapGet("/{id}", async (int id, CrewOpsDbContext db) =>
        {
            var job = await db.Jobs.FindAsync(id);
            return job is not null ? Results.Ok(job) : Results.NotFound();
        })
        .WithName("GetJob")
        .WithOpenApi();

        // POST - Create a new job
        group.MapPost("/", async (Job job, CrewOpsDbContext db) =>
        {
            job.CreatedAt = DateTime.UtcNow;
            db.Jobs.Add(job);
            await db.SaveChangesAsync();
            return Results.Created($"/api/jobs/{job.Id}", job);
        })
        .WithName("CreateJob")
        .WithOpenApi();

        // PUT - Update an existing job
        group.MapPut("/{id}", async (int id, Job updatedJob, CrewOpsDbContext db) =>
        {
            var job = await db.Jobs.FindAsync(id);
            if (job is null) return Results.NotFound();

            job.ReferenceNumber = updatedJob.ReferenceNumber;
            job.Description = updatedJob.Description;
            job.Location = updatedJob.Location;
            job.Status = updatedJob.Status;
            job.StartDate = updatedJob.StartDate;
            job.EndDate = updatedJob.EndDate;

            await db.SaveChangesAsync();
            return Results.Ok(job);
        })
        .WithName("UpdateJob")
        .WithOpenApi();

        // DELETE - Remove a job
        group.MapDelete("/{id}", async (int id, CrewOpsDbContext db) =>
        {
            var job = await db.Jobs.FindAsync(id);
            if (job is null) return Results.NotFound();

            db.Jobs.Remove(job);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithName("DeleteJob")
        .WithOpenApi();

        // ============================================
        // ASSIGNMENT ENDPOINTS
        // ============================================

        // POST - Assign a crew member to a job
        group.MapPost("/{id}/assign", async (int id, AssignCrewRequest request, CrewOpsDbContext db) =>
        {
            // Check if job exists
            var job = await db.Jobs.FindAsync(id);
            if (job is null) return Results.NotFound("Job not found");

            // Check if crew member exists
            var crewMember = await db.CrewMembers.FindAsync(request.CrewMemberId);
            if (crewMember is null) return Results.NotFound("Crew member not found");

            // Check if already assigned
            var existingAssignment = await db.JobAssignments
                .FirstOrDefaultAsync(ja => ja.JobId == id && ja.CrewMemberId == request.CrewMemberId);
            if (existingAssignment is not null)
                return Results.Conflict("Crew member is already assigned to this job");

            // Create assignment
            var assignment = new JobAssignment
            {
                JobId = id,
                CrewMemberId = request.CrewMemberId,
                Role = request.Role,
                AssignedOn = DateTime.UtcNow
            };

            db.JobAssignments.Add(assignment);
            await db.SaveChangesAsync();

            return Results.Ok(new
            {
                assignment.JobId,
                assignment.CrewMemberId,
                assignment.Role,
                assignment.AssignedOn
            });
        })
        .WithName("AssignCrewToJob")
        .WithOpenApi();

        // GET - Get all crew members assigned to a job
        group.MapGet("/{id}/crew", async (int id, CrewOpsDbContext db) =>
        {
            var job = await db.Jobs.FindAsync(id);
            if (job is null) return Results.NotFound("Job not found");

            // Query assignments and map to DTO to avoid circular references
            var crew = await db.JobAssignments
                .Where(ja => ja.JobId == id)
                .Include(ja => ja.CrewMember)
                .Select(ja => new
                {
                    ja.CrewMember.Id,
                    ja.CrewMember.FirstName,
                    ja.CrewMember.LastName,
                    ja.CrewMember.Email,
                    ja.Role,
                    ja.AssignedOn
                })
                .ToListAsync();

            return Results.Ok(crew);
        })
        .WithName("GetJobCrew")
        .WithOpenApi();

        return group;
    }
}

// Request DTO for assigning crew
public record AssignCrewRequest(int CrewMemberId, string? Role);
