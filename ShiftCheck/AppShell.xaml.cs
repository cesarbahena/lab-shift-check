using ShiftCheck.Views;

namespace ShiftCheck;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
		Routing.RegisterRoute(nameof(PendingSamplesPage), typeof(PendingSamplesPage));
		Routing.RegisterRoute(nameof(CreateHandoverPage), typeof(CreateHandoverPage));
	}
}
