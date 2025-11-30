using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using CrewOps.API.Data;
using CrewOps.API.Models;

namespace CrewOps.API.Endpoints;

public static class TimeEntryEndpoints
{
    public static RouteGroupBuilder MapTimeEntryEndpoints(this RouteGroupBuilder group)
    {
        // POST /api/time/clockin - Requires authentication
        group.MapPost("/clockin", async (ClockInRequest request, ClaimsPrincipal user, CrewOpsDbContext db) =>
        {
            // Extract user ID from JWT claims
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null)
                return Results.Unauthorized();

            int crewMemberId = int.Parse(userIdClaim.Value);

            // Check if job exists
            var job = await db.Jobs.FindAsync(request.JobId);
            if (job is null)
                return Results.NotFound("Job not found");

            // CRITICAL: Check if crew member already has an active time entry
            var activeEntry = await db.TimeEntries
                .FirstOrDefaultAsync(te => te.CrewMemberId == crewMemberId && te.ClockOutTime == null);

            if (activeEntry is not null)
                return Results.BadRequest("Already clocked in. Please clock out first.");

            // Create new time entry
            var timeEntry = new TimeEntry
            {
                CrewMemberId = crewMemberId,
                JobId = request.JobId,
                ClockInTime = DateTime.UtcNow,
                Notes = request.Notes
            };

            db.TimeEntries.Add(timeEntry);
            await db.SaveChangesAsync();

            return Results.Ok(new
            {
                timeEntry.Id,
                timeEntry.CrewMemberId,
                timeEntry.JobId,
                timeEntry.ClockInTime,
                timeEntry.Notes,
                Message = "Clocked in successfully"
            });
        })
        .WithName("ClockIn")
        .WithOpenApi()
        .RequireAuthorization();

        // POST /api/time/clockout - Requires authentication
        group.MapPost("/clockout", async (ClockOutRequest request, ClaimsPrincipal user, CrewOpsDbContext db) =>
        {
            // Extract user ID from JWT claims
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null)
                return Results.Unauthorized();

            int crewMemberId = int.Parse(userIdClaim.Value);

            // Find active time entry for this crew member
            var activeEntry = await db.TimeEntries
                .Include(te => te.Job)
                .FirstOrDefaultAsync(te => te.CrewMemberId == crewMemberId && te.ClockOutTime == null);

            if (activeEntry is null)
                return Results.BadRequest("No active shift found. Please clock in first.");

            // Clock out
            activeEntry.ClockOutTime = DateTime.UtcNow;

            // Append notes if provided
            if (!string.IsNullOrWhiteSpace(request.Notes))
            {
                activeEntry.Notes = string.IsNullOrWhiteSpace(activeEntry.Notes)
                    ? request.Notes
                    : $"{activeEntry.Notes} | {request.Notes}";
            }

            await db.SaveChangesAsync();

            var duration = activeEntry.ClockOutTime.Value - activeEntry.ClockInTime;

            return Results.Ok(new
            {
                activeEntry.Id,
                activeEntry.CrewMemberId,
                activeEntry.JobId,
                JobReference = activeEntry.Job.ReferenceNumber,
                activeEntry.ClockInTime,
                activeEntry.ClockOutTime,
                activeEntry.Notes,
                Duration = new
                {
                    Hours = (int)duration.TotalHours,
                    Minutes = duration.Minutes,
                    TotalMinutes = (int)duration.TotalMinutes,
                    Formatted = $"{(int)duration.TotalHours}h {duration.Minutes}m"
                },
                Message = "Clocked out successfully"
            });
        })
        .WithName("ClockOut")
        .WithOpenApi()
        .RequireAuthorization();

        // GET /api/time/history - Get own history (authenticated)
        group.MapGet("/history", async (ClaimsPrincipal user, CrewOpsDbContext db) =>
        {
            // Extract user ID from JWT claims
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null)
                return Results.Unauthorized();

            int crewMemberId = int.Parse(userIdClaim.Value);

            var crewMember = await db.CrewMembers.FindAsync(crewMemberId);
            if (crewMember is null)
                return Results.NotFound("Crew member not found");

            // Get completed time entries (where ClockOutTime is not null)
            var history = await db.TimeEntries
                .Where(te => te.CrewMemberId == crewMemberId && te.ClockOutTime != null)
                .Include(te => te.Job)
                .OrderByDescending(te => te.ClockInTime)
                .Select(te => new
                {
                    te.Id,
                    te.JobId,
                    JobReference = te.Job.ReferenceNumber,
                    JobDescription = te.Job.Description,
                    te.ClockInTime,
                    te.ClockOutTime,
                    te.Notes,
                    DurationMinutes = te.ClockOutTime != null
                        ? (int)(te.ClockOutTime.Value - te.ClockInTime).TotalMinutes
                        : 0
                })
                .ToListAsync();

            return Results.Ok(new
            {
                CrewMemberId = crewMemberId,
                CrewMemberName = $"{crewMember.FirstName} {crewMember.LastName}",
                TotalEntries = history.Count,
                TotalMinutesWorked = history.Sum(h => h.DurationMinutes),
                Entries = history
            });
        })
        .WithName("GetTimeHistory")
        .WithOpenApi()
        .RequireAuthorization();

        // GET /api/time/active - Check if currently clocked in (authenticated)
        group.MapGet("/active", async (ClaimsPrincipal user, CrewOpsDbContext db) =>
        {
            // Extract user ID from JWT claims
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null)
                return Results.Unauthorized();

            int crewMemberId = int.Parse(userIdClaim.Value);

            var activeEntry = await db.TimeEntries
                .Include(te => te.Job)
                .FirstOrDefaultAsync(te => te.CrewMemberId == crewMemberId && te.ClockOutTime == null);

            if (activeEntry is null)
                return Results.Ok(new { IsActive = false });

            var elapsed = DateTime.UtcNow - activeEntry.ClockInTime;

            return Results.Ok(new
            {
                IsActive = true,
                activeEntry.Id,
                activeEntry.JobId,
                JobReference = activeEntry.Job.ReferenceNumber,
                activeEntry.ClockInTime,
                ElapsedMinutes = (int)elapsed.TotalMinutes,
                ElapsedFormatted = $"{(int)elapsed.TotalHours}h {elapsed.Minutes}m"
            });
        })
        .WithName("GetActiveShift")
        .WithOpenApi()
        .RequireAuthorization();

        return group;
    }
}

// Request DTOs - no longer need CrewMemberId (comes from JWT)
public record ClockInRequest(int JobId, string? Notes);
public record ClockOutRequest(string? Notes);
