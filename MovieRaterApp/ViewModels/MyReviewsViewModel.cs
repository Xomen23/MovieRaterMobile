using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieRaterApp.Models;
using MovieRaterApp.Services;
using MovieRaterApp.Views;

namespace MovieRaterApp.ViewModels;

public partial class MyReviewsViewModel : ObservableObject
{
    private readonly ApiService _apiService;
    private readonly SessionService _sessionService;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _errorMessage;

    public ObservableCollection<Review> MyReviews { get; } = [];

    public MyReviewsViewModel(ApiService apiService, SessionService sessionService)
    {
        _apiService = apiService;
        _sessionService = sessionService;
    }

    [RelayCommand]
    private async Task LoadMyReviewsAsync()
    {
        var user = _sessionService.GetSavedUser();
        if (user?.Id is null)
        {
            ErrorMessage = "Niste prijavljeni.";
            return;
        }

        try
        {
            IsLoading = true;
            ErrorMessage = null;

            // Review model sadrži samo imdbId, ne i naziv filma — prikazujemo imdbId dok backend ne vrati i title.
            var reviews = await _apiService.GetUserReviewsAsync(user.Id);

            MyReviews.Clear();
            if (reviews is not null)
            {
                foreach (var review in reviews)
                    MyReviews.Add(review);
            }
        }
        catch
        {
            ErrorMessage = "Učitavanje recenzija nije uspelo.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task GoToMovieDetailsAsync(Review? review)
    {
        if (string.IsNullOrWhiteSpace(review?.ImdbId))
            return;

        await Shell.Current.GoToAsync($"{nameof(MovieDetailsPage)}?imdbId={Uri.EscapeDataString(review.ImdbId)}");
    }

    [RelayCommand]
    private async Task DeleteReviewAsync(Review? review)
    {
        if (review?.Id is null)
            return;

        var confirmed = await Shell.Current.DisplayAlert(
            "Brisanje recenzije",
            "Da li sigurno želiš da obrišeš ovu recenziju?",
            "Da",
            "Ne");

        if (!confirmed)
            return;

        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var success = await _apiService.DeleteReviewAsync(review.Id);
            if (!success)
            {
                ErrorMessage = "Brisanje recenzije nije uspelo.";
                return;
            }

            await LoadMyReviewsAsync();
        }
        catch
        {
            ErrorMessage = "Brisanje recenzije nije uspelo.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task GoToProfileAsync()
    {
        await Shell.Current.GoToAsync(nameof(ProfilePage));
    }
}
