using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShiftCheck.Services;
using ShiftCheck.Views;

namespace ShiftCheck.ViewModels;

public partial class LoginViewModel : ObservableObject
{
	private readonly IAuthService _authService;

	[ObservableProperty]
	private string username = string.Empty;

	[ObservableProperty]
	private string password = string.Empty;

	[ObservableProperty]
	private bool isBusy;

	[ObservableProperty]
	private string errorMessage = string.Empty;

	public LoginViewModel(IAuthService authService)
	{
		_authService = authService;
	}

	[RelayCommand]
	async Task LoginAsync()
	{
		if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
		{
			ErrorMessage = "Por favor ingrese usuario y contraseña";
			return;
		}

		IsBusy = true;
		ErrorMessage = string.Empty;

		try
		{
			var result = await _authService.LoginAsync(Username, Password);

			if (result != null)
			{
				await Shell.Current.GoToAsync($"//{nameof(PendingSamplesPage)}");
			}
			else
			{
				ErrorMessage = "Usuario o contraseña incorrectos";
			}
		}
		catch (Exception ex)
		{
			ErrorMessage = $"Error de conexión: {ex.Message}";
		}
		finally
		{
			IsBusy = false;
		}
	}
}
