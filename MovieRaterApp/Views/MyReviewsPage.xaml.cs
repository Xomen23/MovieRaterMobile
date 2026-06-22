namespace MovieRaterApp.Views;

public partial class MyReviewsPage : ContentPage
{
    public MyReviewsPage()
    {
        InitializeComponent();
    }

    private async void OnGoBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
