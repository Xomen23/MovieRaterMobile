using MovieRaterApp.ViewModels;

namespace MovieRaterApp.Views;

public partial class MyReviewsPage : ContentPage
{
    private readonly MyReviewsViewModel _viewModel;

    public MyReviewsPage(MyReviewsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadMyReviewsCommand.Execute(null);
    }
}
