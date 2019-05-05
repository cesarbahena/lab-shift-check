using ShiftCheck.Services;
using ShiftCheck.Views;

namespace ShiftCheck;

public partial class App : Application
{
	private readonly IAuthService _authService;

	public App(IAuthService authService)
	{
		InitializeComponent();
		_authService = authService;

		SetMainPage();
	}

	private async void SetMainPage()
	{
		var isAuthenticated = await _authService.IsAuthenticatedAsync();
		MainPage = isAuthenticated
			? new AppShell()
			: new NavigationPage(new LoginPage());
	}
}
