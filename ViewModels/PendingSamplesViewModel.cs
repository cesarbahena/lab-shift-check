using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using ShiftCheck.Models;
using ShiftCheck.Services;
using ShiftCheck.Views;

namespace ShiftCheck.ViewModels
{

public partial class PendingSamplesViewModel : ViewModelBase
{
	private readonly IApiService _apiService;
	private readonly IAuthService _authService;
	private readonly ILogger<PendingSamplesViewModel> _logger;

	private ObservableCollection<SampleDto> _samples = new ObservableCollection<SampleDto>();
	public Command LoadUserCommand { get { return new Command(async () => await LoadUserAsync()); } }
	public Command LoadSamplesCommand { get { return new Command(async () => await LoadSamplesAsync()); } }
	public Command<SampleDto> ToggleSampleCommand { get { return new Command<SampleDto>(item => ToggleSample(item)); } }
	public Command CreateHandoverCommand { get { return new Command(async () => await CreateHandoverAsync()); } }
	public Command LogoutCommand { get { return new Command(async () => await LogoutAsync()); } }

	public ObservableCollection<SampleDto> Samples { get { return _samples; } set { SetProperty(ref _samples, value); } }

	private ObservableCollection<SampleDto> _selectedSamples = new ObservableCollection<SampleDto>();
	public ObservableCollection<SampleDto> SelectedSamples { get { return _selectedSamples; } set { SetProperty(ref _selectedSamples, value); } }

	private bool _isBusy;
	public bool IsBusy { get { return _isBusy; } set { if (SetProperty(ref _isBusy, value)) OnPropertyChanged(nameof(CanCreateHandover)); } }

	private string _currentUserName = string.Empty;
	public string CurrentUserName { get { return _currentUserName; } set { SetProperty(ref _currentUserName, value); } }

	private string _loadError = string.Empty;
	public string LoadError { get { return _loadError; } set { if (SetProperty(ref _loadError, value)) OnPropertyChanged(nameof(CanCreateHandover)); } }
	public bool CanCreateHandover { get { return !IsBusy && string.IsNullOrEmpty(LoadError) && SelectedSamples.Count > 0; } }

	private string _emptyMessage = string.Empty;
	public string EmptyMessage { get { return _emptyMessage; } set { SetProperty(ref _emptyMessage, value); } }

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

	async Task LoadSamplesAsync()
	{
		IsBusy = true;
		LoadError = string.Empty;
		EmptyMessage = string.Empty;
		try
		{
			_logger.LogInformation("Loading pending samples");
			var pendingSamples = await _apiService.GetPendingSamplesAsync();
			var selectedIds = new HashSet<int>(SelectedSamples.Select(sample => sample.Id));

			SelectedSamples.Clear();
			Samples.Clear();
			foreach (var sample in pendingSamples)
			{
				sample.IsSelected = selectedIds.Contains(sample.Id);
				Samples.Add(sample);
				if (sample.IsSelected)
					SelectedSamples.Add(sample);
			}

			if (Samples.Count == 0)
				EmptyMessage = "No hay exámenes pendientes.";
			OnPropertyChanged(nameof(CanCreateHandover));

			_logger.LogInformation("Loaded {Count} pending samples", Samples.Count);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error loading pending samples");
			LoadError = Samples.Count == 0
				? "No se pudieron cargar los exámenes pendientes. Reintente la carga."
				: "No se pudo actualizar. Se muestran los exámenes de la última carga; reintente antes de preparar la entrega.";
			OnPropertyChanged(nameof(CanCreateHandover));
		}
		finally
		{
			IsBusy = false;
		}
	}

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
		OnPropertyChanged(nameof(CanCreateHandover));
	}

	async Task CreateHandoverAsync()
	{
		_logger.LogInformation("Creating handover with {Count} selected samples", SelectedSamples.Count);

		if (!CanCreateHandover)
		{
			_logger.LogWarning("Cannot create handover: no samples selected");
			await Shell.Current.DisplayAlert("Error", "Seleccione al menos una muestra pendiente", "OK");
			return;
		}

		_logger.LogDebug("Navigating to CreateHandoverPage");
		await Shell.Current.GoToAsync(nameof(CreateHandoverPage));
	}

	async Task LogoutAsync()
	{
		_logger.LogInformation("User logout initiated");
		await _authService.LogoutAsync();
		_logger.LogDebug("Navigating to LoginPage");
		await Shell.Current.GoToAsync("..");
	}
}

}
