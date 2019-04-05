using ShiftCheck.Views;

namespace ShiftCheck;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		MainPage = new NavigationPage(new LoginPage());
	}
}
