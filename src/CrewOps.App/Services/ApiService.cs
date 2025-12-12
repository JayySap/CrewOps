using System.Net.Http.Json;
using CrewOps.Shared.DTOs;

namespace CrewOps.App.Services;

/// <summary>
/// Service for communicating with the CrewOps API.
/// Teacher's Note: This service handles all HTTP communication with the backend.
/// It uses:
/// - HttpClient for making HTTP requests
/// - System.Net.Http.Json for easy JSON serialization/deserialization
/// - SecureStorage for safely storing sensitive data like auth tokens
/// </summary>
public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private const string AuthTokenKey = "auth_token";

    /// <summary>
    /// Constructor - HttpClient is injected via Dependency Injection.
    /// Teacher's Note: We don't create HttpClient ourselves because:
    /// 1. DI manages its lifetime properly (avoids socket exhaustion)
    /// 2. We can configure it once in MauiProgram.cs (base address, headers)
    /// </summary>
    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Attempts to log in with the provided credentials.
    /// </summary>
    public async Task<bool> LoginAsync(string email, string password)
    {
        try
        {
            // Create the request DTO (from CrewOps.Shared)
            var loginRequest = new LoginRequest(email, password);

            System.Diagnostics.Debug.WriteLine($"Attempting login to: {_httpClient.BaseAddress}api/auth/login");
            System.Diagnostics.Debug.WriteLine($"Email: {email}");

            // POST to the login endpoint
            // PostAsJsonAsync automatically serializes the object to JSON
            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", loginRequest);

            System.Diagnostics.Debug.WriteLine($"Response status: {response.StatusCode}");

            // Check if the request was successful (HTTP 200-299)
            if (response.IsSuccessStatusCode)
            {
                // Parse the response to get the token
                // ReadFromJsonAsync automatically deserializes JSON to object
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

                if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                {
                    // Store the token
                    // Teacher's Note: Using Preferences for development simplicity.
                    // In production, use SecureStorage with proper entitlements configured.
                    // SecureStorage requires keychain entitlements on MacCatalyst/iOS.
                    Preferences.Default.Set(AuthTokenKey, loginResponse.Token);
                    System.Diagnostics.Debug.WriteLine("Login successful, token stored");
                    return true;
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Login failed: {errorContent}");
            }

            return false;
        }
        catch (HttpRequestException ex)
        {
            // Network error - can't reach the API
            throw new Exception($"Cannot connect to API at {_httpClient.BaseAddress}. Is the API running? Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Login error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets the stored authentication token.
    /// </summary>
    public Task<string?> GetAuthTokenAsync()
    {
        var token = Preferences.Default.Get<string?>(AuthTokenKey, null);
        return Task.FromResult(token);
    }

    /// <summary>
    /// Clears the stored authentication token (logout).
    /// </summary>
    public Task LogoutAsync()
    {
        Preferences.Default.Remove(AuthTokenKey);
        return Task.CompletedTask;
    }
}
