namespace CrewOps.API.Models;

/// <summary>
/// Join entity representing the assignment of a CrewMember to a Job.
/// Uses a composite primary key (JobId + CrewMemberId) to prevent duplicate assignments.
/// </summary>
public class JobAssignment
{
    // Foreign Keys
    public int JobId { get; set; }
    public int CrewMemberId { get; set; }

    // Assignment details
    public DateTime AssignedOn { get; set; } = DateTime.UtcNow;
    public string? Role { get; set; } // e.g., "Lead", "Member"

    // Navigation properties
    public Job Job { get; set; } = null!;
    public CrewMember CrewMember { get; set; } = null!;
}
