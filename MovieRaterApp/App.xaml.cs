namespace MovieRaterApp;

using MovieRaterApp.Services;

public partial class App : Application
{
	public App(AppShell appShell, ApiService apiService)
	{
		InitializeComponent();

		MainPage = appShell;
		_ = apiService.EnsureConnectionAsync();
	}
}
