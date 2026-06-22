namespace MovieRaterApp.Views;

public partial class MovieDetailsPage : ContentPage
{
    public MovieDetailsPage()
    {
        InitializeComponent();
    }

    private async void OnGoBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
