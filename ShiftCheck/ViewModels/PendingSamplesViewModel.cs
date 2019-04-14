using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShiftCheck.Models;
using ShiftCheck.Services;
using ShiftCheck.Views;

namespace ShiftCheck.ViewModels;

public partial class PendingSamplesViewModel : ObservableObject
{
	private readonly IApiService _apiService;
	private readonly IAuthService _authService;

	[ObservableProperty]
	private ObservableCollection<SampleDto> samples = new();

	[ObservableProperty]
	private ObservableCollection<SampleDto> selectedSamples = new();

	[ObservableProperty]
	private bool isBusy;

	[ObservableProperty]
	private string currentUserName = string.Empty;

	public PendingSamplesViewModel(IApiService apiService, IAuthService authService)
	{
		_apiService = apiService;
		_authService = authService;
	}

	public async Task InitializeAsync()
	{
		await LoadUserAsync();
		await LoadSamplesAsync();
	}

	[RelayCommand]
	async Task LoadUserAsync()
	{
		var user = await _authService.GetCurrentUserAsync();
		if (user != null)
		{
			CurrentUserName = user.FullName;
		}
	}

	[RelayCommand]
	async Task LoadSamplesAsync()
	{
		IsBusy = true;
		try
		{
			var pendingSamples = await _apiService.GetPendingSamplesAsync();
			Samples.Clear();
			foreach (var sample in pendingSamples)
			{
				Samples.Add(sample);
			}
		}
		finally
		{
			IsBusy = false;
		}
	}

	[RelayCommand]
	void ToggleSample(SampleDto sample)
	{
		if (SelectedSamples.Contains(sample))
		{
			SelectedSamples.Remove(sample);
		}
		else
		{
			SelectedSamples.Add(sample);
		}
	}

	[RelayCommand]
	async Task CreateHandoverAsync()
	{
		if (SelectedSamples.Count == 0)
		{
			await Shell.Current.DisplayAlert("Error", "Seleccione al menos una muestra pendiente", "OK");
			return;
		}

		await Shell.Current.GoToAsync(nameof(CreateHandoverPage));
	}

	[RelayCommand]
	async Task LogoutAsync()
	{
		await _authService.LogoutAsync();
		await Shell.Current.GoToAsync("//LoginPage");
	}
}
