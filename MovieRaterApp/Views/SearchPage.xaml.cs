namespace MovieRaterApp.Views;

public partial class SearchPage : ContentPage
{
    public SearchPage()
    {
        InitializeComponent();
    }

    private async void OnGoToMovieDetailsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(MovieDetailsPage));
    }

    private async void OnGoToMyReviewsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(MyReviewsPage));
    }

    private async void OnGoBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
