using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrewOps.App.Services;

namespace CrewOps.App.ViewModels;

/// <summary>
/// ViewModel for the Login page.
/// Teacher's Note: This class follows the MVVM (Model-View-ViewModel) pattern:
/// - Model: The data (email, password, API responses)
/// - View: The UI (LoginPage.xaml)
/// - ViewModel: The glue that connects them (this class)
///
/// Benefits of MVVM:
/// 1. Separation of concerns - UI logic separate from business logic
/// 2. Testability - ViewModels can be unit tested without UI
/// 3. Maintainability - Changes to UI don't affect logic and vice versa
///
/// CommunityToolkit.Mvvm provides attributes that generate boilerplate code:
/// - [ObservableProperty] generates property with INotifyPropertyChanged
/// - [RelayCommand] generates ICommand implementation
/// </summary>
public partial class LoginViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    /// <summary>
    /// Constructor - IApiService is injected via Dependency Injection.
    /// </summary>
    public LoginViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    /// <summary>
    /// The user's email address.
    /// Teacher's Note: [ObservableProperty] is a source generator attribute.
    /// It automatically generates:
    /// - A public property "Email" (from the private field "email")
    /// - PropertyChanged notifications when the value changes
    ///
    /// The generated code looks something like:
    /// public string Email
    /// {
    ///     get => email;
    ///     set => SetProperty(ref email, value);
    /// }
    /// </summary>
    [ObservableProperty]
    private string email = string.Empty;

    /// <summary>
    /// The user's password.
    /// </summary>
    [ObservableProperty]
    private string password = string.Empty;

    /// <summary>
    /// Indicates whether a login operation is in progress.
    /// Used to show loading indicator and disable the login button.
    /// </summary>
    [ObservableProperty]
    private bool isBusy;

    /// <summary>
    /// Error message to display to the user.
    /// </summary>
    [ObservableProperty]
    private string errorMessage = string.Empty;

    /// <summary>
    /// Attempts to log in with the provided credentials.
    /// Teacher's Note: [RelayCommand] generates an ICommand property named "LoginCommand".
    /// The UI binds to this command, and when the button is clicked, this method runs.
    ///
    /// The "async Task" signature means:
    /// - The command will be awaited properly
    /// - The toolkit handles disabling the command while it's running
    /// </summary>
    [RelayCommand]
    private async Task Login()
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(Email))
        {
            ErrorMessage = "Please enter your email";
            return;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Please enter your password";
            return;
        }

        // Clear any previous error
        ErrorMessage = string.Empty;

        // Show loading state
        IsBusy = true;

        try
        {
            // Attempt to log in
            var success = await _apiService.LoginAsync(Email, Password);

            if (success)
            {
                // Show success message
                // Teacher's Note: In a real app, you'd navigate to the main page here
                await Application.Current!.MainPage!.DisplayAlert(
                    "Success",
                    "Login successful!",
                    "OK");

                // TODO: Navigate to main page
                // await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                ErrorMessage = "Invalid email or password";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred: {ex.Message}";
        }
        finally
        {
            // Always hide loading state
            IsBusy = false;
        }
    }
}
