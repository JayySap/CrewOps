namespace CrewOps.API.Models;

/// <summary>
/// Represents a time tracking entry for a crew member working on a job.
/// ClockOutTime is null when the crew member is currently clocked in.
/// </summary>
public class TimeEntry
{
    public int Id { get; set; }

    // Foreign Keys
    public int CrewMemberId { get; set; }
    public int JobId { get; set; }

    // Time tracking
    public DateTime ClockInTime { get; set; }
    public DateTime? ClockOutTime { get; set; } // null = currently working

    public string? Notes { get; set; }

    // Navigation properties
    public CrewMember CrewMember { get; set; } = null!;
    public Job Job { get; set; } = null!;

    // Computed property for duration (only valid when clocked out)
    public TimeSpan? Duration => ClockOutTime.HasValue
        ? ClockOutTime.Value - ClockInTime
        : null;
}
