using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using ShiftCheck.Models;
using ShiftCheck.Services;
using ShiftCheck.Views;

namespace ShiftCheck.ViewModels;

public partial class PendingSamplesViewModel : ObservableObject
{
	private readonly IApiService _apiService;
	private readonly IAuthService _authService;
	private readonly ILogger<PendingSamplesViewModel> _logger;

	[ObservableProperty]
	private ObservableCollection<SampleDto> samples = new();

	[ObservableProperty]
	private ObservableCollection<SampleDto> selectedSamples = new();

	[ObservableProperty]
	private bool isBusy;

	[ObservableProperty]
	private string currentUserName = string.Empty;

	[ObservableProperty]
	private string loadError = string.Empty;

	[ObservableProperty]
	private string emptyMessage = string.Empty;

	public PendingSamplesViewModel(IApiService apiService, IAuthService authService, ILogger<PendingSamplesViewModel> logger)
	{
		_apiService = apiService;
		_authService = authService;
		_logger = logger;
		_logger.LogInformation("PendingSamplesViewModel initialized");
	}

	public async Task InitializeAsync()
	{
		_logger.LogInformation("Initializing PendingSamplesViewModel");
		await LoadUserAsync();
		await LoadSamplesAsync();
	}

	[RelayCommand]
	async Task LoadUserAsync()
	{
		try
		{
			_logger.LogDebug("Loading current user information");
			var user = await _authService.GetCurrentUserAsync();

			if (user != null)
			{
				CurrentUserName = user.FullName;
				_logger.LogInformation("Current user loaded: {UserName}", user.FullName);
			}
			else
			{
				CurrentUserName = "Usuario";
				_logger.LogWarning("No user information available, using default");
			}
		}
		catch (Exception ex)
		{
			CurrentUserName = "Usuario";
			_logger.LogError(ex, "Error loading user information");
		}
	}

	[RelayCommand]
	async Task LoadSamplesAsync()
	{
		IsBusy = true;
		LoadError = string.Empty;
		EmptyMessage = string.Empty;
		try
		{
			_logger.LogInformation("Loading pending samples");
			var pendingSamples = await _apiService.GetPendingSamplesAsync();

			SelectedSamples.Clear();
			Samples.Clear();
			foreach (var sample in pendingSamples)
			{
				sample.IsSelected = false;
				Samples.Add(sample);
			}

			if (Samples.Count == 0)
				EmptyMessage = "No hay exámenes pendientes.";

			_logger.LogInformation("Loaded {Count} pending samples", Samples.Count);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error loading pending samples");
			Samples.Clear();
			SelectedSamples.Clear();
			LoadError = "No se pudieron cargar los exámenes pendientes. Deslice hacia abajo para reintentar.";
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
			sample.IsSelected = false;
			_logger.LogDebug("Sample {SampleId} deselected, total selected: {Count}", sample.Id, SelectedSamples.Count);
		}
		else
		{
			SelectedSamples.Add(sample);
			sample.IsSelected = true;
			_logger.LogDebug("Sample {SampleId} selected, total selected: {Count}", sample.Id, SelectedSamples.Count);
		}
	}

	[RelayCommand]
	async Task CreateHandoverAsync()
	{
		_logger.LogInformation("Creating handover with {Count} selected samples", SelectedSamples.Count);

		if (SelectedSamples.Count == 0)
		{
			_logger.LogWarning("Cannot create handover: no samples selected");
			await Shell.Current.DisplayAlert("Error", "Seleccione al menos una muestra pendiente", "OK");
			return;
		}

		_logger.LogDebug("Navigating to CreateHandoverPage");
		await Shell.Current.GoToAsync(nameof(CreateHandoverPage));
	}

	[RelayCommand]
	async Task LogoutAsync()
	{
		_logger.LogInformation("User logout initiated");
		await _authService.LogoutAsync();
		_logger.LogDebug("Navigating to LoginPage");
		await Shell.Current.GoToAsync("//LoginPage");
	}
}
