using CrewOps.Shared.DTOs;

namespace CrewOps.App.Services;

/// <summary>
/// Interface for API communication service.
/// Teacher's Note: Using an interface allows for:
/// 1. Easy mocking in unit tests
/// 2. Swapping implementations (e.g., mock service for offline mode)
/// 3. Dependency injection - the app depends on abstraction, not concrete class
/// </summary>
public interface IApiService
{
    /// <summary>
    /// Attempts to log in with the provided credentials.
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <param name="password">User's password</param>
    /// <returns>True if login succeeded, false otherwise</returns>
    Task<bool> LoginAsync(string email, string password);

    /// <summary>
    /// Gets the stored authentication token, if any.
    /// </summary>
    Task<string?> GetAuthTokenAsync();

    /// <summary>
    /// Clears the stored authentication token (logout).
    /// </summary>
    Task LogoutAsync();
}
