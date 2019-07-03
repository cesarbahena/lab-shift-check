using ShiftCheck.ViewModels;
using Xamarin.Forms;

namespace ShiftCheck.Views
{

public partial class PendingSamplesPage : ContentPage
{
	private readonly PendingSamplesViewModel _viewModel;
	public PendingSamplesPage() : this(AppBootstrapper.Get<PendingSamplesViewModel>()) { }

	public PendingSamplesPage(PendingSamplesViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
		_viewModel = viewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _viewModel.InitializeAsync();
	}
}

}
