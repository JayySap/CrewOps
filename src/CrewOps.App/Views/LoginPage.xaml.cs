using CrewOps.App.ViewModels;

namespace CrewOps.App.Views;

/// <summary>
/// Code-behind for the Login page.
/// Teacher's Note: In MVVM, the code-behind should be minimal.
/// Most logic lives in the ViewModel. The code-behind only:
/// 1. Sets up the BindingContext (connects View to ViewModel)
/// 2. Handles UI-specific things that can't be done in XAML
///
/// The ViewModel is injected via the constructor (Dependency Injection).
/// </summary>
public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();

        // Connect this page to its ViewModel
        // Teacher's Note: BindingContext is how XAML bindings know where to find data.
        // All {Binding PropertyName} expressions look in BindingContext for properties.
        BindingContext = viewModel;
    }
}
