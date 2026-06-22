using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieRaterApp.Models;
using MovieRaterApp.Services;

namespace MovieRaterApp.ViewModels;

[QueryProperty(nameof(ImdbId), "imdbId")]
public partial class MovieDetailsViewModel : ObservableObject, IQueryAttributable
{
    private readonly ApiService _apiService;
    private readonly SessionService _sessionService;

    [ObservableProperty]
    private string _imdbId = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MovieTitle))]
    [NotifyPropertyChangedFor(nameof(MovieYear))]
    [NotifyPropertyChangedFor(nameof(MoviePlot))]
    [NotifyPropertyChangedFor(nameof(MovieImageUrl))]
    private ImdbTitleResponse? _movieDetails;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AverageRatingDisplay))]
    private AverageRatingResponse? _averageRating;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEditingReview))]
    [NotifyPropertyChangedFor(nameof(SubmitReviewButtonText))]
    [NotifyPropertyChangedFor(nameof(HasMyReview))]
    private Review? _myReview;

    [ObservableProperty]
    private int _myRatingInput = 5;

    [ObservableProperty]
    private string _myReviewTextInput = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanSubmitReview))]
    private bool _isSubmitting;

    [ObservableProperty]
    private string? _errorMessage;

    public ObservableCollection<Review> Reviews { get; } = [];

    public bool IsEditingReview => MyReview is not null;

    public bool HasMyReview => MyReview is not null;

    public bool CanSubmitReview => !IsSubmitting;

    public string SubmitReviewButtonText => IsEditingReview ? "Ažuriraj" : "Sačuvaj";

    public string MovieTitle => MovieDetails?.PrimaryTitle ?? "Film";

    public string MovieYear => MovieDetails?.StartYear?.ToString() ?? string.Empty;

    public string MoviePlot => MovieDetails?.Plot ?? string.Empty;

    public string? MovieImageUrl => MovieDetails?.PrimaryImage?.Url;

    public string AverageRatingDisplay =>
        AverageRating is null
            ? "Nema ocena"
            : $"★ {AverageRating.AverageRating:F1} ({AverageRating.ReviewCount} {(AverageRating.ReviewCount == 1 ? "recenzija" : "recenzije")})";

    public MovieDetailsViewModel(ApiService apiService, SessionService sessionService)
    {
        _apiService = apiService;
        _sessionService = sessionService;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("imdbId", out var value))
            ImdbId = Uri.UnescapeDataString(value?.ToString() ?? string.Empty);
    }

    partial void OnImdbIdChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            _ = LoadDataAsync();
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        if (string.IsNullOrWhiteSpace(ImdbId))
            return;

        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var detailsTask = _apiService.GetMovieDetailsAsync(ImdbId);
            var averageTask = _apiService.GetAverageRatingAsync(ImdbId);
            var reviewsTask = _apiService.GetMovieReviewsAsync(ImdbId);

            await Task.WhenAll(detailsTask, averageTask, reviewsTask);

            MovieDetails = await detailsTask;
            AverageRating = await averageTask;
            var reviews = await reviewsTask;

            Reviews.Clear();
            if (reviews is not null)
            {
                foreach (var review in reviews)
                    Reviews.Add(review);
            }

            ApplyMyReviewFromReviews();

            if (MovieDetails is null)
                ErrorMessage = "Neuspelo učitavanje detalja filma.";
        }
        catch
        {
            ErrorMessage = "Neuspelo učitavanje podataka. Proveri konekciju.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SubmitReviewAsync()
    {
        var rating = MyRatingInput;
        if (rating is < 1 or > 10)
        {
            ErrorMessage = "Ocena mora biti između 1 i 10.";
            return;
        }

        var user = _sessionService.GetSavedUser();
        if (user?.Id is null)
        {
            ErrorMessage = "Morate biti prijavljeni da biste ostavili recenziju.";
            return;
        }

        try
        {
            IsSubmitting = true;
            ErrorMessage = null;

            bool success;

            if (MyReview is null)
            {
                if (MovieDetails is null)
                {
                    ErrorMessage = "Detalji filma nisu učitani.";
                    return;
                }

                var review = new Review
                {
                    UserId = user.Id,
                    Username = user.Username,
                    ImdbId = ImdbId,
                    Rating = rating,
                    ReviewText = MyReviewTextInput
                };

                var movie = MapToMovie(MovieDetails);
                success = await _apiService.PostReviewAsync(new ReviewRequest
                {
                    Review = review,
                    Movie = movie
                }) is not null;
            }
            else
            {
                var updated = await _apiService.UpdateReviewAsync(MyReview.Id!, new UpdateReviewRequest
                {
                    UserId = user.Id,
                    Rating = rating,
                    ReviewText = MyReviewTextInput
                });
                success = updated is not null;
            }

            if (!success)
            {
                ErrorMessage = "Čuvanje recenzije nije uspelo.";
                return;
            }

            await Shell.Current.DisplayAlert("Uspeh", "Recenzija sačuvana.", "OK");
            await LoadDataAsync();
        }
        catch
        {
            ErrorMessage = "Čuvanje recenzije nije uspelo.";
        }
        finally
        {
            IsSubmitting = false;
        }
    }

    [RelayCommand]
    private async Task DeleteMyReviewAsync()
    {
        if (MyReview?.Id is null)
            return;

        var confirmed = await Shell.Current.DisplayAlert(
            "Brisanje recenzije",
            "Da li sigurno želiš da obrišeš svoju recenziju?",
            "Da",
            "Ne");

        if (!confirmed)
            return;

        try
        {
            IsSubmitting = true;
            ErrorMessage = null;

            var success = await _apiService.DeleteReviewAsync(MyReview.Id);
            if (!success)
            {
                ErrorMessage = "Brisanje recenzije nije uspelo.";
                return;
            }

            await LoadDataAsync();
            MyRatingInput = 5;
            MyReviewTextInput = string.Empty;
        }
        catch
        {
            ErrorMessage = "Brisanje recenzije nije uspelo.";
        }
        finally
        {
            IsSubmitting = false;
        }
    }

    private void ApplyMyReviewFromReviews()
    {
        var userId = _sessionService.GetSavedUser()?.Id;
        MyReview = string.IsNullOrWhiteSpace(userId)
            ? null
            : Reviews.FirstOrDefault(r => r.UserId == userId);

        if (MyReview is not null)
        {
            MyRatingInput = MyReview.Rating ?? 5;
            MyReviewTextInput = MyReview.ReviewText ?? string.Empty;
        }
        else
        {
            MyRatingInput = 5;
            MyReviewTextInput = string.Empty;
        }
    }

    private static Movie MapToMovie(ImdbTitleResponse details) => new()
    {
        Id = details.Id,
        Title = details.PrimaryTitle,
        ImageUrl = details.PrimaryImage?.Url,
        Description = details.Plot,
        Year = details.StartYear
    };
}
