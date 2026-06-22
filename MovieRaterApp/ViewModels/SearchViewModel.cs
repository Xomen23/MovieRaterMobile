using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieRaterApp.Models;
using MovieRaterApp.Services;
using MovieRaterApp.Views;

namespace MovieRaterApp.ViewModels;

public partial class SearchViewModel : ObservableObject
{
    private readonly ApiService _apiService;
    private readonly SessionService _sessionService;

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowNoResults))]
    private bool _isLoading;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowNoResults))]
    private bool _hasSearched;

    [ObservableProperty]
    private string? _errorMessage;

    public ObservableCollection<ImdbTitle> SearchResults { get; } = [];

    public bool ShowNoResults => HasSearched && !IsLoading && SearchResults.Count == 0;

    public SearchViewModel(ApiService apiService, SessionService sessionService)
    {
        _apiService = apiService;
        _sessionService = sessionService;
        SearchResults.CollectionChanged += (_, _) => OnPropertyChanged(nameof(ShowNoResults));
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
            return;

        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var response = await _apiService.SearchMoviesAsync(SearchQuery.Trim());

            SearchResults.Clear();

            if (response?.Titles is { Count: > 0 })
            {
                foreach (var title in response.Titles)
                    SearchResults.Add(title);
            }
            else if (response is null)
            {
                ErrorMessage = "Pretraga nije uspela, proveri konekciju.";
            }
        }
        finally
        {
            HasSearched = true;
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task GoToMovieDetailsAsync(ImdbTitle? movie)
    {
        if (string.IsNullOrWhiteSpace(movie?.Id))
            return;

        await Shell.Current.GoToAsync($"{nameof(MovieDetailsPage)}?imdbId={Uri.EscapeDataString(movie.Id)}");
    }

    [RelayCommand]
    private async Task GoToMyReviewsAsync()
    {
        await Shell.Current.GoToAsync(nameof(MyReviewsPage));
    }

    [RelayCommand]
    private async Task GoToProfileAsync()
    {
        await Shell.Current.GoToAsync(nameof(ProfilePage));
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        _sessionService.ClearSession();
        await Shell.Current.GoToAsync("//LoginPage");
    }
}
