namespace CrewOps.App;

/// <summary>
/// Application-wide constants.
/// Teacher's Note: Centralizing configuration values makes them easy to find and change.
/// In production, you might load these from appsettings.json or environment variables.
/// </summary>
public static class Constants
{
    /// <summary>
    /// Base URL for the CrewOps API.
    /// Using HTTP for local development to avoid SSL certificate issues.
    /// Change to HTTPS with proper cert in production.
    /// </summary>
    // Using 127.0.0.1 instead of localhost for MacCatalyst compatibility
    public static string ApiUrl = "http://127.0.0.1:5152";
}
