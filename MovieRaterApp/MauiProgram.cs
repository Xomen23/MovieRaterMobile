using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using MovieRaterApp.Services;
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

		// Android emulator mapira 10.0.2.2 na localhost host mašine.
		// Za fizički telefon koristi IP računara u lokalnoj mreži (npr. http://192.168.x.x:8080/).
		builder.Services.AddSingleton(_ =>
		{
			var httpClient = new HttpClient
			{
				BaseAddress = new Uri("http://10.0.2.2:8080/")
			};
			return httpClient;
		});
		builder.Services.AddSingleton<ApiService>();
		builder.Services.AddSingleton<SessionService>();

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
