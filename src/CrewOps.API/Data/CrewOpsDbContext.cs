using Microsoft.EntityFrameworkCore;
using CrewOps.Shared.Models;

namespace CrewOps.API.Data;

/// <summary>
/// The DbContext is the "bridge" between your C# code and the database.
/// Think of it as a session with the database - you use it to query and save data.
/// </summary>
public class CrewOpsDbContext : DbContext
{
    // Constructor - receives configuration (like connection string) via dependency injection
    public CrewOpsDbContext(DbContextOptions<CrewOpsDbContext> options) : base(options)
    {
    }

    // DbSet = a table in the database
    // This creates a "CrewMembers" table based on the CrewMember entity
    public DbSet<CrewMember> CrewMembers => Set<CrewMember>();

    // Jobs table
    public DbSet<Job> Jobs => Set<Job>();

    // JobAssignments table (join table for Many-to-Many)
    public DbSet<JobAssignment> JobAssignments => Set<JobAssignment>();

    // TimeEntries table
    public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();

    // OnModelCreating lets you configure how entities map to database tables
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure the CrewMember entity
        modelBuilder.Entity<CrewMember>(entity =>
        {
            // Make FirstName required with max length
            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            // Make LastName required with max length
            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);

            // Email is optional but has max length
            entity.Property(e => e.Email)
                .HasMaxLength(200);

            // Status has a default value
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Active");
        });

        // Configure the Job entity
        modelBuilder.Entity<Job>(entity =>
        {
            entity.Property(e => e.ReferenceNumber)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.Location)
                .IsRequired()
                .HasMaxLength(200);

            // Store enum as string for readability in SQLite
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(JobStatus.Pending);
        });

        // Configure the JobAssignment join entity
        modelBuilder.Entity<JobAssignment>(entity =>
        {
            // Composite primary key - prevents duplicate assignments
            entity.HasKey(ja => new { ja.JobId, ja.CrewMemberId });

            // Relationship: JobAssignment -> Job
            entity.HasOne(ja => ja.Job)
                .WithMany(j => j.Assignments)
                .HasForeignKey(ja => ja.JobId)
                .OnDelete(DeleteBehavior.Cascade); // Delete assignments when job is deleted

            // Relationship: JobAssignment -> CrewMember
            entity.HasOne(ja => ja.CrewMember)
                .WithMany(c => c.Assignments)
                .HasForeignKey(ja => ja.CrewMemberId)
                .OnDelete(DeleteBehavior.Cascade); // Delete assignments when crew member is deleted

            entity.Property(ja => ja.Role)
                .HasMaxLength(50);
        });

        // Configure the TimeEntry entity
        modelBuilder.Entity<TimeEntry>(entity =>
        {
            entity.Property(e => e.ClockInTime)
                .IsRequired();

            entity.Property(e => e.Notes)
                .HasMaxLength(500);

            // Relationship: TimeEntry -> CrewMember
            entity.HasOne(te => te.CrewMember)
                .WithMany()
                .HasForeignKey(te => te.CrewMemberId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship: TimeEntry -> Job
            entity.HasOne(te => te.Job)
                .WithMany()
                .HasForeignKey(te => te.JobId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
