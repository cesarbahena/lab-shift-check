using ShiftCheck.ViewModels;
using Xamarin.Forms;

namespace ShiftCheck.Views
{

public partial class CreateHandoverPage : ContentPage
{
	private readonly CreateHandoverViewModel _viewModel;
	public CreateHandoverPage() : this(AppBootstrapper.Get<CreateHandoverViewModel>()) { }

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

}
