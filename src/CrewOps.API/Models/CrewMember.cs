namespace CrewOps.API.Models;

/// <summary>
/// Represents a field crew member in the system.
/// This class is an "entity" - it maps directly to a database table.
/// </summary>
public class CrewMember
{
    // Primary Key - EF Core recognizes "Id" or "CrewMemberId" as the primary key by convention
    public int Id { get; set; }

    // Required field - we'll configure this in DbContext
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    // Optional field (nullable string)
    public string? Email { get; set; }

    // Example of a simple status - could be "Active", "OnLeave", "Inactive"
    public string Status { get; set; } = "Active";

    // When this crew member was added to the system
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
