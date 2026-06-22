using MovieRaterApp.Views;

namespace MovieRaterApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
        Routing.RegisterRoute(nameof(MovieDetailsPage), typeof(MovieDetailsPage));
        Routing.RegisterRoute(nameof(MyReviewsPage), typeof(MyReviewsPage));
    }
}
