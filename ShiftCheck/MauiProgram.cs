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
#endif

		builder.Services.AddSingleton<IAuthService, AuthService>();
		builder.Services.AddSingleton<IApiService, ApiService>();

		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<LoginViewModel>();

		builder.Services.AddTransient<PendingSamplesPage>();
		builder.Services.AddTransient<PendingSamplesViewModel>();

		builder.Services.AddTransient<CreateHandoverPage>();
		builder.Services.AddTransient<CreateHandoverViewModel>();

		builder.Services.AddHttpClient("QuimiOSHub", client =>
		{
			client.BaseAddress = new Uri("http://localhost:5000");
			client.Timeout = TimeSpan.FromSeconds(30);
		});

		return builder.Build();
	}
}
