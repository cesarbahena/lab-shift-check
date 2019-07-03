using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Microsoft.Extensions.Logging;
using ShiftCheck.Services;

namespace ShiftCheck.ViewModels
{

public partial class LoginViewModel : ViewModelBase
{
	private readonly IAuthService _authService;
	private readonly ILogger<LoginViewModel> _logger;

	private string _username = string.Empty;
	public Command LoginCommand { get { return new Command(async () => await LoginAsync()); } }

	public string Username { get { return _username; } set { SetProperty(ref _username, value); } }

	private string _password = string.Empty;
	public string Password { get { return _password; } set { SetProperty(ref _password, value); } }

	private string _hubUrl = HubConnectionSettings.BaseUrl;
	public string HubUrl { get { return _hubUrl; } set { SetProperty(ref _hubUrl, value); } }

	private bool _isBusy;
	public bool IsBusy { get { return _isBusy; } set { SetProperty(ref _isBusy, value); } }

	private string _errorMessage = string.Empty;
	public string ErrorMessage { get { return _errorMessage; } set { SetProperty(ref _errorMessage, value); } }

	public LoginViewModel(IAuthService authService, ILogger<LoginViewModel> logger)
	{
		_authService = authService;
		_logger = logger;
		_logger.LogInformation("LoginViewModel initialized");
	}

	async Task LoginAsync()
	{
		_logger.LogInformation("Login attempt started for username: {Username}", Username);

		if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
		{
			ErrorMessage = "Ingrese usuario y contraseña";
			_logger.LogWarning("Login validation failed: empty credentials");
			return;
		}

		if (!HubConnectionSettings.TryNormalize(HubUrl, out var normalizedUrl))
		{
			ErrorMessage = "Ingrese la URL del Hub, con ruta /api.";
			return;
		}

		IsBusy = true;
		ErrorMessage = string.Empty;
		HubConnectionSettings.BaseUrl = normalizedUrl;

		try
		{
			var result = await _authService.LoginAsync(Username, Password);

			if (result != null)
			{
				_logger.LogInformation("Login successful, navigating to PendingSamplesPage");
				await Shell.Current.GoToAsync("PendingSamplesPage");
			}
			else
			{
				ErrorMessage = "Credenciales inválidas";
				_logger.LogWarning("Login failed: invalid credentials");
			}
		}
		catch (Exception ex)
		{
			ErrorMessage = ex.Message;
			_logger.LogError(ex, "Login failed with exception");
		}
		finally
		{
			IsBusy = false;
			_logger.LogDebug("Login attempt completed, IsBusy set to false");
		}
	}
}

}
