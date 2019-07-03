using ShiftCheck.ViewModels;
using Xamarin.Forms;

namespace ShiftCheck.Views
{

public partial class LoginPage : ContentPage
{
	public LoginPage() : this(AppBootstrapper.Get<LoginViewModel>()) { }

	public LoginPage(LoginViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}

}
