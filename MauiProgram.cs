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
		builder.Logging.AddDebug();
		builder.Logging.SetMinimumLevel(LogLevel.Debug);
#else
		builder.Logging.SetMinimumLevel(LogLevel.Information);
#endif

		ConfigureServices(builder.Services);

		return builder.Build();
	}

	private static void ConfigureServices(IServiceCollection services)
	{
		services.AddHttpClient("QuimiOSHub", client =>
		{
			client.BaseAddress = new Uri("https://quimioshub-production.up.railway.app/api/");
			client.Timeout = TimeSpan.FromSeconds(30);
		});

		services.AddSingleton<IAuthService, AuthService>();
		services.AddSingleton<IApiService, ApiService>();

		services.AddTransient<LoginPage>();
		services.AddTransient<LoginViewModel>();

		services.AddTransient<PendingSamplesPage>();
		services.AddSingleton<PendingSamplesViewModel>();

		services.AddTransient<CreateHandoverPage>();
		services.AddTransient<CreateHandoverViewModel>();

		services.AddTransient<AppShell>();
	}
}
