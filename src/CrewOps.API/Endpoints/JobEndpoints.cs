using Microsoft.EntityFrameworkCore;
using CrewOps.API.Data;
using CrewOps.API.Models;

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

        return group;
    }
}
