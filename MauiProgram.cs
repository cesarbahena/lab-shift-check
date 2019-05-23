using Microsoft.Extensions.Logging;
using ShiftCheck.Services;
using ShiftCheck.ViewModels;
using ShiftCheck.Views;

namespace ShiftCheck;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		// Debug logging available via Microsoft.Extensions.Logging.Debug package
#endif

		ConfigureServices(builder.Services);

		return builder.Build();
	}

	private static void ConfigureServices(IServiceCollection services)
	{
		services.AddSingleton<IAuthService, AuthService>();
		services.AddSingleton<IApiService, ApiService>();

		services.AddTransient<LoginPage>();
		services.AddTransient<LoginViewModel>();

		services.AddTransient<PendingSamplesPage>();
		services.AddTransient<PendingSamplesViewModel>();

		services.AddTransient<CreateHandoverPage>();
		services.AddTransient<CreateHandoverViewModel>();

		services.AddHttpClient("QuimiOSHub", client =>
		{
			client.BaseAddress = new Uri("http://localhost:5000/api/");
			client.Timeout = TimeSpan.FromSeconds(30);
			client.DefaultRequestHeaders.Add("Accept", "application/json");
		});
	}
}
