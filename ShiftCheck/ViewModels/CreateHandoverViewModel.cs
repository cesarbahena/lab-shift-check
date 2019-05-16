using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShiftCheck.Models;
using ShiftCheck.Services;

namespace ShiftCheck.ViewModels;

public partial class CreateHandoverViewModel : ObservableObject
{
	private readonly IApiService _apiService;
	private readonly IAuthService _authService;
	private readonly PendingSamplesViewModel _pendingSamplesViewModel;

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
		PendingSamplesViewModel pendingSamplesViewModel)
	{
		_apiService = apiService;
		_authService = authService;
		_pendingSamplesViewModel = pendingSamplesViewModel;
	}

	public async Task InitializeAsync()
	{
		await LoadShiftsAsync();
	}

	[RelayCommand]
	async Task LoadShiftsAsync()
	{
		IsBusy = true;
		try
		{
			var shiftList = await _apiService.GetShiftsAsync();
			Shifts.Clear();
			foreach (var shift in shiftList)
			{
				Shifts.Add(shift);
			}

			if (Shifts.Count > 0)
			{
				SelectedShift = Shifts[0];
			}
			else
			{
				await Shell.Current.DisplayAlert("Error", "No se encontraron turnos disponibles", "OK");
			}
		}
		catch (Exception ex)
		{
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
		if (SelectedShift == null)
		{
			await Shell.Current.DisplayAlert("Error", "Seleccione un turno", "OK");
			return;
		}

		if (_pendingSamplesViewModel.SelectedSamples.Count == 0)
		{
			await Shell.Current.DisplayAlert("Error", "No hay muestras seleccionadas", "OK");
			return;
		}

		IsBusy = true;

		try
		{
			var currentUser = await _authService.GetCurrentUserAsync();
			if (currentUser == null)
			{
				await Shell.Current.DisplayAlert("Error", "Usuario no autenticado", "OK");
				return;
			}

			var pendingSamples = _pendingSamplesViewModel.SelectedSamples
				.Select(s => new PendingSampleDto
				{
					SampleId = s.Id,
					FolioGrd = s.FolioGrd,
					Reason = "Pendiente de liberación"
				})
				.ToList();

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
				await Shell.Current.DisplayAlert("Éxito", "Entrega de turno creada correctamente", "OK");
				_pendingSamplesViewModel.SelectedSamples.Clear();
				await Shell.Current.GoToAsync("..");
			}
			else
			{
				await Shell.Current.DisplayAlert("Error", "No se pudo crear la entrega de turno", "OK");
			}
		}
		catch (Exception ex)
		{
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
		await Shell.Current.GoToAsync("..");
	}
}
