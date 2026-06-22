namespace MovieRaterApp.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnGoToRegisterClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(RegisterPage));
    }

    private async void OnGoToSearchClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SearchPage));
    }
}
