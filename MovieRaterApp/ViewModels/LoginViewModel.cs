using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieRaterApp.Models;
using MovieRaterApp.Services;
using MovieRaterApp.Views;

namespace MovieRaterApp.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly ApiService _apiService;
    private readonly SessionService _sessionService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanLogin))]
    private bool _isLoading;

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string? _errorMessage;

    public bool CanLogin => !IsLoading;

    public LoginViewModel(ApiService apiService, SessionService sessionService)
    {
        _apiService = apiService;
        _sessionService = sessionService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Unesite korisničko ime i lozinku.";
            return;
        }

        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var user = await _apiService.LoginAsync(new LoginRequest
            {
                Username = Username.Trim(),
                Password = Password
            });

            if (user is not null)
            {
                _sessionService.SaveUser(user);
                Password = string.Empty;
                await Shell.Current.GoToAsync("//SearchPage");
            }
            else
            {
                ErrorMessage = "Pogrešno korisničko ime ili lozinka";
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task GoToRegisterAsync()
    {
        await Shell.Current.GoToAsync(nameof(RegisterPage));
    }
}
