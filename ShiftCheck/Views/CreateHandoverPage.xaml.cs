using ShiftCheck.ViewModels;

namespace ShiftCheck.Views;

public partial class CreateHandoverPage : ContentPage
{
	private readonly CreateHandoverViewModel _viewModel;

	public CreateHandoverPage(CreateHandoverViewModel viewModel)
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
