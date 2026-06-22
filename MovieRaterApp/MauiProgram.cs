using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using MovieRaterApp.Services;
using MovieRaterApp.ViewModels;
using MovieRaterApp.Views;

namespace MovieRaterApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddSingleton<ApiConnectionResolver>();
		builder.Services.AddSingleton(_ => new HttpClient
		{
			Timeout = TimeSpan.FromSeconds(15)
		});
		builder.Services.AddSingleton<ApiService>();
		builder.Services.AddSingleton<SessionService>();

		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<RegisterViewModel>();
		builder.Services.AddTransient<SearchViewModel>();
		builder.Services.AddTransient<MovieDetailsViewModel>();
		builder.Services.AddTransient<MyReviewsViewModel>();
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<RegisterPage>();
		builder.Services.AddTransient<SearchPage>();
		builder.Services.AddTransient<MovieDetailsPage>();
		builder.Services.AddTransient<MyReviewsPage>();
		builder.Services.AddSingleton<AppShell>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
