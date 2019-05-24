using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using ShiftCheck.Models;
using ShiftCheck.Services;

namespace ShiftCheck.ViewModels;

public partial class CreateHandoverViewModel : ObservableObject
{
	private readonly IApiService _apiService;
	private readonly IAuthService _authService;
	private readonly PendingSamplesViewModel _pendingSamplesViewModel;
	private readonly ILogger<CreateHandoverViewModel> _logger;

	[ObservableProperty]
	private ObservableCollection<ShiftDto> shifts = new();

	[ObservableProperty]
	private ShiftDto? selectedShift;

	[ObservableProperty]
	private string notes = string.Empty;

	[ObservableProperty]
	private bool isBusy;

	[ObservableProperty]
	private DateTime handoverDate = DateTime.Now;

	public CreateHandoverViewModel(
		IApiService apiService,
		IAuthService authService,
		PendingSamplesViewModel pendingSamplesViewModel,
		ILogger<CreateHandoverViewModel> logger)
	{
		_apiService = apiService;
		_authService = authService;
		_pendingSamplesViewModel = pendingSamplesViewModel;
		_logger = logger;
		_logger.LogInformation("CreateHandoverViewModel initialized");
	}

	public async Task InitializeAsync()
	{
		_logger.LogInformation("Initializing CreateHandoverViewModel");
		await LoadShiftsAsync();
	}

	[RelayCommand]
	async Task LoadShiftsAsync()
	{
		IsBusy = true;
		try
		{
			_logger.LogInformation("Loading shifts");
			var shiftList = await _apiService.GetShiftsAsync();

			Shifts.Clear();
			foreach (var shift in shiftList)
			{
				Shifts.Add(shift);
			}

			_logger.LogInformation("Loaded {Count} shifts", Shifts.Count);

			if (Shifts.Count > 0)
			{
				SelectedShift = Shifts[0];
				_logger.LogDebug("Default shift selected: {ShiftName}", SelectedShift.Name);
			}
			else
			{
				_logger.LogWarning("No shifts available");
				await Shell.Current.DisplayAlert("Error", "No se encontraron turnos disponibles", "OK");
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error loading shifts");
			await Shell.Current.DisplayAlert("Error", $"Error al cargar turnos: {ex.Message}", "OK");
		}
		finally
		{
			IsBusy = false;
		}
	}

	[RelayCommand]
	async Task SaveHandoverAsync()
	{
		_logger.LogInformation("Saving handover");

		if (SelectedShift == null)
		{
			_logger.LogWarning("Cannot save handover: no shift selected");
			await Shell.Current.DisplayAlert("Error", "Seleccione un turno", "OK");
			return;
		}

		if (_pendingSamplesViewModel.SelectedSamples.Count == 0)
		{
			_logger.LogWarning("Cannot save handover: no samples selected");
			await Shell.Current.DisplayAlert("Error", "No hay muestras seleccionadas", "OK");
			return;
		}

		IsBusy = true;

		try
		{
			_logger.LogDebug("Getting current user");
			var currentUser = await _authService.GetCurrentUserAsync();

			if (currentUser == null)
			{
				_logger.LogError("Cannot save handover: user not authenticated");
				await Shell.Current.DisplayAlert("Error", "Usuario no autenticado", "OK");
				return;
			}

			var pendingSamples = _pendingSamplesViewModel.SelectedSamples
				.Select(s => new PendingSampleDto
				{
					SampleId = s.Id,
					Folio = s.Folio,
					Reason = "Pendiente de liberación"
				})
				.ToList();

			_logger.LogInformation("Creating handover for shift {ShiftId} with {SampleCount} samples",
				SelectedShift.Id, pendingSamples.Count);

			var handover = new CreateShiftHandoverDto
			{
				ShiftId = SelectedShift.Id,
				UserId = currentUser.Id,
				HandoverDate = HandoverDate,
				Notes = Notes,
				PendingSamples = pendingSamples
			};

			var result = await _apiService.CreateShiftHandoverAsync(handover);

			if (result != null)
			{
				_logger.LogInformation("Handover created successfully with ID {HandoverId}", result.Id);
				await Shell.Current.DisplayAlert("Éxito", "Entrega de turno creada correctamente", "OK");
				_pendingSamplesViewModel.SelectedSamples.Clear();
				await Shell.Current.GoToAsync("..");
			}
			else
			{
				_logger.LogWarning("Failed to create handover: API returned null");
				await Shell.Current.DisplayAlert("Error", "No se pudo crear la entrega de turno", "OK");
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error creating handover");
			await Shell.Current.DisplayAlert("Error", $"Error al crear entrega: {ex.Message}", "OK");
		}
		finally
		{
			IsBusy = false;
		}
	}

	[RelayCommand]
	async Task CancelAsync()
	{
		_logger.LogInformation("Handover creation cancelled");
		await Shell.Current.GoToAsync("..");
	}
}
