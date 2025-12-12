namespace CrewOps.Shared.DTOs;

// Request DTOs for time tracking - no longer need CrewMemberId (comes from JWT)
public record ClockInRequest(int JobId, string? Notes);
public record ClockOutRequest(string? Notes);
