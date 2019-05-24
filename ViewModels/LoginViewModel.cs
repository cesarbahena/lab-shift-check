using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using ShiftCheck.Services;

namespace ShiftCheck.ViewModels;

public partial class LoginViewModel : ObservableObject
{
	private readonly IAuthService _authService;
	private readonly ILogger<LoginViewModel> _logger;

	[ObservableProperty]
	private string username = string.Empty;

	[ObservableProperty]
	private string password = string.Empty;

	[ObservableProperty]
	private bool isBusy;

	[ObservableProperty]
	private string errorMessage = string.Empty;

	public LoginViewModel(IAuthService authService, ILogger<LoginViewModel> logger)
	{
		_authService = authService;
		_logger = logger;
		_logger.LogInformation("LoginViewModel initialized");
	}

	[RelayCommand]
	async Task LoginAsync()
	{
		_logger.LogInformation("Login attempt started for username: {Username}", Username);

		if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
		{
			ErrorMessage = "Ingrese usuario y contraseña";
			_logger.LogWarning("Login validation failed: empty credentials");
			return;
		}

		IsBusy = true;
		ErrorMessage = string.Empty;

		try
		{
			var result = await _authService.LoginAsync(Username, Password);

			if (result != null)
			{
				_logger.LogInformation("Login successful, navigating to PendingSamplesPage");
				await Shell.Current.GoToAsync($"//PendingSamplesPage");
			}
			else
			{
				ErrorMessage = "Credenciales inválidas";
				_logger.LogWarning("Login failed: invalid credentials");
			}
		}
		catch (Exception ex)
		{
			ErrorMessage = $"Error de conexión: {ex.Message}";
			_logger.LogError(ex, "Login failed with exception");
		}
		finally
		{
			IsBusy = false;
			_logger.LogDebug("Login attempt completed, IsBusy set to false");
		}
	}
}
