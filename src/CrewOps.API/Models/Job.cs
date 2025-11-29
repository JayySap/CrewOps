namespace CrewOps.API.Models;

public class Job
{
    public int Id { get; set; }

    // e.g., "JOB-2024-001"
    public string ReferenceNumber { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public JobStatus Status { get; set; } = JobStatus.Pending;

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property - crew members assigned to this job
    public List<JobAssignment> Assignments { get; set; } = [];
}
