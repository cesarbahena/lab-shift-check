using ShiftCheck.Services;
using ShiftCheck.Views;

namespace ShiftCheck;

public partial class App : Application
{
	public App(IServiceProvider serviceProvider)
	{
		InitializeComponent();

		// Set MainPage immediately to avoid crash
		MainPage = new NavigationPage(serviceProvider.GetRequiredService<LoginPage>());
	}
}
