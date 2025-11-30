using CrewOps.API.Models;

namespace CrewOps.API.Data;

public static class DbInitializer
{
    public static void Initialize(CrewOpsDbContext context)
    {
        // Check if any users exist
        if (context.CrewMembers.Any())
        {
            return; // Database already seeded
        }

        // Create admin user with hashed password
        var admin = new CrewMember
        {
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@crewops.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("AdminPassword123!"),
            Role = "Admin",
            Status = "Active",
            CreatedAt = DateTime.UtcNow
        };

        context.CrewMembers.Add(admin);
        context.SaveChanges();
    }
}
