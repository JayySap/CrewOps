using Microsoft.Extensions.Logging;
using CrewOps.App.Services;
using CrewOps.App.ViewModels;
using CrewOps.App.Views;

namespace CrewOps.App;

/// <summary>
/// Application entry point and configuration.
/// Teacher's Note: This is where we configure Dependency Injection (DI).
/// DI is a pattern where:
/// 1. Classes declare their dependencies in the constructor
/// 2. The framework automatically creates and provides those dependencies
/// 3. This makes code more testable and loosely coupled
///
/// Lifetime options:
/// - Singleton: One instance for the entire app lifetime
/// - Scoped: One instance per "scope" (less common in MAUI)
/// - Transient: New instance every time it's requested
/// </summary>
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // ============================================
        // DEPENDENCY INJECTION REGISTRATION
        // ============================================

        // Register HttpClient with base address
        // Teacher's Note: We configure HttpClient once here, and it's reused.
        // The BaseAddress means we can use relative URLs in ApiService.
        builder.Services.AddSingleton(sp =>
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(Constants.ApiUrl)
            };
            return client;
        });

        // Register Services
        // Teacher's Note: Singleton means one instance shared across the app.
        // Good for stateless services like ApiService.
        builder.Services.AddSingleton<IApiService, ApiService>();

        // Register ViewModels
        // Teacher's Note: Transient means new instance each time.
        // Good for ViewModels - each page gets its own fresh ViewModel.
        builder.Services.AddTransient<LoginViewModel>();

        // Register Pages
        // Teacher's Note: Pages are Transient so each navigation creates fresh page.
        builder.Services.AddTransient<LoginPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
