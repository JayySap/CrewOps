namespace CrewOps.Shared.DTOs;

// Request/Response DTOs for authentication
public record LoginRequest(string Email, string Password);
public record LoginResponse(string Token, int UserId, string Name, string Role);
public record SetPasswordRequest(int CrewMemberId, string Password);
