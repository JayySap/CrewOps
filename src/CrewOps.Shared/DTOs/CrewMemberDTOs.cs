namespace CrewOps.Shared.DTOs;

// Request DTO for creating crew members
public record CreateCrewMemberRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string? Role
);

// Request DTO for updating crew members (no password - use set-password endpoint)
public record UpdateCrewMemberRequest(
    string FirstName,
    string LastName,
    string Email,
    string Status
);
