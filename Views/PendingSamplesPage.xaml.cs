using ShiftCheck.ViewModels;

namespace ShiftCheck.Views;

public partial class PendingSamplesPage : ContentPage
{
	private readonly PendingSamplesViewModel _viewModel;

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
