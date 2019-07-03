using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using ShiftCheck.Models;
using ShiftCheck.Services;

namespace ShiftCheck.ViewModels
{

public partial class CreateHandoverViewModel : ViewModelBase
{
	private readonly IApiService _apiService;
	private readonly IAuthService _authService;
	private readonly PendingSamplesViewModel _pendingSamplesViewModel;
	private readonly ILogger<CreateHandoverViewModel> _logger;

	private ObservableCollection<ShiftDto> _shifts = new ObservableCollection<ShiftDto>();
	public Command LoadShiftsCommand { get { return new Command(async () => await LoadShiftsAsync()); } }
	public Command SaveHandoverCommand { get { return new Command(async () => await SaveHandoverAsync()); } }
	public Command CancelCommand { get { return new Command(async () => await CancelAsync()); } }

	public ObservableCollection<ShiftDto> Shifts { get { return _shifts; } set { SetProperty(ref _shifts, value); } }

	private ObservableCollection<HandoverItem> _pendingItems = new ObservableCollection<HandoverItem>();
	public ObservableCollection<HandoverItem> PendingItems { get { return _pendingItems; } set { SetProperty(ref _pendingItems, value); } }

	private ShiftDto _selectedShift;
	public ShiftDto SelectedShift { get { return _selectedShift; } set { SetProperty(ref _selectedShift, value); } }

	private string _notes = string.Empty;
	public string Notes { get { return _notes; } set { SetProperty(ref _notes, value); } }

	private bool _isBusy;
	public bool IsBusy { get { return _isBusy; } set { SetProperty(ref _isBusy, value); } }

	private DateTime _handoverDate = DateTime.Now;
	public DateTime HandoverDate { get { return _handoverDate; } set { SetProperty(ref _handoverDate, value); } }

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
		PendingItems.Clear();
		foreach (var sample in _pendingSamplesViewModel.SelectedSamples)
		{
			PendingItems.Add(new HandoverItem
			{
				ExamId = sample.Id,
				Folio = sample.Folio,
				ExamName = sample.ExamName ?? string.Empty
			});
		}
		await LoadShiftsAsync();
	}

	async Task LoadShiftsAsync()
	{
		IsBusy = true;
		SelectedShift = null;
		Shifts.Clear();
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

	async Task SaveHandoverAsync()
	{
		_logger.LogInformation("Saving handover");

		if (SelectedShift == null)
		{
			_logger.LogWarning("Cannot save handover: no shift selected");
			await Shell.Current.DisplayAlert("Error", "Seleccione un turno", "OK");
			return;
		}

		if (PendingItems.Count == 0)
		{
			_logger.LogWarning("Cannot save handover: no samples selected");
			await Shell.Current.DisplayAlert("Error", "No hay muestras seleccionadas", "OK");
			return;
		}

		if (PendingItems.Any(item => string.IsNullOrWhiteSpace(item.Reason)))
		{
			await Shell.Current.DisplayAlert("Faltan motivos", "Escriba un motivo para cada examen pendiente.", "OK");
			return;
		}

		var summary = string.Join(", ", PendingItems.Take(8).Select(item => item.Folio.HasValue
			? item.Folio.Value.ToString() : item.ExamId.ToString()));
		if (PendingItems.Count > 8)
			summary += $" y {PendingItems.Count - 8} más";
		var confirmed = await Shell.Current.DisplayAlert("Revisar entrega",
			$"Turno: {SelectedShift.Name}\nFecha: {HandoverDate:dd/MM/yyyy}\nExámenes ({PendingItems.Count}): {summary}",
			"Confirmar", "Volver");
		if (!confirmed)
			return;

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

			var pendingExams = PendingItems
				.Select(item => new PendingSampleDto
				{
					ExamId = item.ExamId,
					Folio = item.Folio,
					Reason = item.Reason.Trim()
				})
				.ToList();

			_logger.LogInformation("Creating handover for shift {ShiftId} with {SampleCount} samples",
				SelectedShift.Id, pendingExams.Count);

			var handover = new CreateShiftHandoverDto
			{
				ShiftId = SelectedShift.Id,
				UserId = currentUser.Id,
				HandoverDate = HandoverDate,
				Notes = Notes,
				PendingExams = pendingExams
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
				await Shell.Current.DisplayAlert("Sin confirmación", "No se pudo confirmar la entrega. Consulta Hub antes de reintentar para evitar duplicados.", "OK");
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error creating handover");
			await Shell.Current.DisplayAlert("Sin confirmación", "No se pudo confirmar la entrega. Consulta Hub antes de reintentar para evitar duplicados.", "OK");
		}
		finally
		{
			IsBusy = false;
		}
	}

	async Task CancelAsync()
	{
		_logger.LogInformation("Handover creation cancelled");
		await Shell.Current.GoToAsync("..");
	}
}

}
