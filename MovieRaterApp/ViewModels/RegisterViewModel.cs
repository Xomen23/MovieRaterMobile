using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieRaterApp.Models;
using MovieRaterApp.Services;

namespace MovieRaterApp.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanRegister))]
    private bool _isLoading;

    [ObservableProperty]
    private string _firstName = string.Empty;

    [ObservableProperty]
    private string _lastName = string.Empty;

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _confirmPassword = string.Empty;

    [ObservableProperty]
    private string? _errorMessage;

    public bool CanRegister => !IsLoading;

    [ObservableProperty]
    private string _serverUrl = "Povezivanje...";

    public RegisterViewModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task RefreshConnectionAsync()
    {
        await _apiService.EnsureConnectionAsync();
        ServerUrl = _apiService.BaseUrl;
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (string.IsNullOrWhiteSpace(FirstName) ||
            string.IsNullOrWhiteSpace(LastName) ||
            string.IsNullOrWhiteSpace(Username) ||
            string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Sva polja su obavezna.";
            return;
        }

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Lozinke se ne poklapaju.";
            return;
        }

        try
        {
            IsLoading = true;
            ErrorMessage = null;
            await RefreshConnectionAsync();

            var (userId, errorMessage) = await _apiService.RegisterAsync(new RegisterRequest
            {
                FirstName = FirstName.Trim(),
                LastName = LastName.Trim(),
                Username = Username.Trim(),
                Password = Password
            });

            if (!string.IsNullOrWhiteSpace(userId))
            {
                await Shell.Current.DisplayAlert("Uspeh", "Registracija je uspešna. Možete se prijaviti.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                ErrorMessage = errorMessage ?? "Registracija nije uspela. Probaj drugo korisničko ime.";
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task GoToLoginAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
