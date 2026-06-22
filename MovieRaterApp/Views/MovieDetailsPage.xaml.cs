using MovieRaterApp.ViewModels;

namespace MovieRaterApp.Views;

public partial class MovieDetailsPage : ContentPage, IQueryAttributable
{
    private readonly MovieDetailsViewModel _viewModel;

    public MovieDetailsPage(MovieDetailsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        _viewModel.ApplyQueryAttributes(query);
    }
}
