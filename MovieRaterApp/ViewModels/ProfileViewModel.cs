using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieRaterApp.Models;
using MovieRaterApp.Services;

namespace MovieRaterApp.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    private readonly ApiService _apiService;
    private readonly SessionService _sessionService;

    [ObservableProperty]
    private string _currentUsername = string.Empty;

    [ObservableProperty]
    private string _newUsername = string.Empty;

    [ObservableProperty]
    private string? _usernameErrorMessage;

    [ObservableProperty]
    private string? _usernameSuccessMessage;

    [ObservableProperty]
    private bool _isUpdatingUsername;

    [ObservableProperty]
    private string _currentPassword = string.Empty;

    [ObservableProperty]
    private string _newPassword = string.Empty;

    [ObservableProperty]
    private string _confirmNewPassword = string.Empty;

    [ObservableProperty]
    private string? _passwordErrorMessage;

    [ObservableProperty]
    private string? _passwordSuccessMessage;

    [ObservableProperty]
    private bool _isUpdatingPassword;

    public ProfileViewModel(ApiService apiService, SessionService sessionService)
    {
        _apiService = apiService;
        _sessionService = sessionService;
    }

    public async Task LoadCurrentUserAsync()
    {
        var user = await ResolveSessionUserAsync();
        CurrentUsername = user?.Username ?? string.Empty;
    }

    private async Task<User?> ResolveSessionUserAsync()
    {
        var user = _sessionService.GetSavedUser();
        if (user is null)
            return null;

        if (!string.IsNullOrWhiteSpace(user.Id))
            return user;

        if (string.IsNullOrWhiteSpace(user.Username))
            return null;

        var fromApi = await _apiService.GetUserAsync(user.Username);
        if (fromApi?.Id is not null)
        {
            _sessionService.SaveUser(fromApi);
            return fromApi;
        }

        return null;
    }

    [RelayCommand]
    private async Task ChangeUsernameAsync()
    {
        UsernameErrorMessage = null;
        UsernameSuccessMessage = null;

        if (string.IsNullOrWhiteSpace(NewUsername))
        {
            UsernameErrorMessage = "Unesite novo korisničko ime.";
            return;
        }

        if (NewUsername.Trim().Equals(CurrentUsername, StringComparison.OrdinalIgnoreCase))
        {
            UsernameErrorMessage = "Novo korisničko ime mora biti različito od trenutnog.";
            return;
        }

        var user = await ResolveSessionUserAsync();
        if (user?.Id is null)
        {
            UsernameErrorMessage = "Niste prijavljeni. Odjavite se i ponovo se ulogujte.";
            return;
        }

        try
        {
            IsUpdatingUsername = true;

            var (updatedUser, errorMessage) = await _apiService.UpdateUsernameAsync(user.Id, NewUsername.Trim());
            if (updatedUser is null)
            {
                UsernameErrorMessage = errorMessage ?? "Promena korisničkog imena nije uspela.";
                return;
            }

            if (string.IsNullOrWhiteSpace(updatedUser.Id))
                updatedUser.Id = user.Id;

            _sessionService.SaveUser(updatedUser);
            CurrentUsername = updatedUser.Username ?? NewUsername.Trim();
            NewUsername = string.Empty;
            UsernameSuccessMessage = "Korisničko ime je promenjeno.";
        }
        finally
        {
            IsUpdatingUsername = false;
        }
    }

    [RelayCommand]
    private async Task ChangePasswordAsync()
    {
        PasswordErrorMessage = null;
        PasswordSuccessMessage = null;

        if (string.IsNullOrWhiteSpace(CurrentPassword) ||
            string.IsNullOrWhiteSpace(NewPassword) ||
            string.IsNullOrWhiteSpace(ConfirmNewPassword))
        {
            PasswordErrorMessage = "Sva polja su obavezna.";
            return;
        }

        if (NewPassword != ConfirmNewPassword)
        {
            PasswordErrorMessage = "Nova lozinka i potvrda se ne poklapaju.";
            return;
        }

        if (NewPassword.Length < 6)
        {
            PasswordErrorMessage = "Nova lozinka mora imati najmanje 6 karaktera.";
            return;
        }

        var user = await ResolveSessionUserAsync();
        if (user?.Id is null)
        {
            PasswordErrorMessage = "Niste prijavljeni. Odjavite se i ponovo se ulogujte.";
            return;
        }

        try
        {
            IsUpdatingPassword = true;

            var (success, errorMessage) = await _apiService.UpdatePasswordAsync(
                user.Id, CurrentPassword, NewPassword);

            if (!success)
            {
                PasswordErrorMessage = errorMessage ?? "Promena lozinke nije uspela.";
                return;
            }

            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmNewPassword = string.Empty;
            PasswordSuccessMessage = "Lozinka je promenjena.";
        }
        finally
        {
            IsUpdatingPassword = false;
        }
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        _sessionService.ClearSession();
        await Shell.Current.GoToAsync("//LoginPage");
    }
}
