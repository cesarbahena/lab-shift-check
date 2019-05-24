using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using ShiftCheck.Models;

namespace ShiftCheck.Services;

public class AuthService : IAuthService
{
	private const string ApiUrl = "https://quimioshub-production.up.railway.app/api/auth/login";
	private const string TokenKey = "auth_token";
	private const string UsernameKey = "auth_username";
	private const string FullNameKey = "auth_fullname";
	private const string RoleKey = "auth_role";
	private const string UserIdKey = "auth_userid";

	private readonly HttpClient _httpClient;
	private readonly ILogger<AuthService> _logger;

	public AuthService(ILogger<AuthService> logger)
	{
		_httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
		_logger = logger;
	}

	public async Task<LoginResponse?> LoginAsync(string username, string password)
	{
		try
		{
			_logger.LogInformation("Starting login for user: {Username}", username);

			var payload = JsonSerializer.Serialize(new { username, password });
			var content = new StringContent(payload, Encoding.UTF8, "application/json");

			_logger.LogDebug("Sending POST request to {ApiUrl}", ApiUrl);
			var response = await _httpClient.PostAsync(ApiUrl, content);

			_logger.LogInformation("Login response status: {StatusCode}", response.StatusCode);

			if (!response.IsSuccessStatusCode)
			{
				_logger.LogWarning("Login failed for user {Username}: {StatusCode}", username, response.StatusCode);
				return null;
			}

			var body = await response.Content.ReadAsStringAsync();
			_logger.LogDebug("Response body length: {Length} characters", body.Length);

			var result = JsonSerializer.Deserialize<LoginResponse>(body,
				new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

			if (result?.Token != null)
			{
				_logger.LogDebug("Storing authentication data in SecureStorage");
				await SecureStorage.Default.SetAsync(TokenKey, result.Token);
				await SecureStorage.Default.SetAsync(UsernameKey, result.Username);
				await SecureStorage.Default.SetAsync(FullNameKey, result.FullName);
				await SecureStorage.Default.SetAsync(RoleKey, result.Role);

				_logger.LogInformation("Login successful for user {Username} ({FullName})", result.Username, result.FullName);
			}
			else
			{
				_logger.LogWarning("Login response did not contain a valid token");
			}

			return result;
		}
		catch (HttpRequestException ex)
		{
			_logger.LogError(ex, "Network error during login for user {Username}", username);
			throw new Exception("Error de conexión. Verifique su conexión a internet.", ex);
		}
		catch (TaskCanceledException ex)
		{
			_logger.LogError(ex, "Login request timeout for user {Username}", username);
			throw new Exception("La solicitud tardó demasiado. Intente nuevamente.", ex);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Unexpected error during login for user {Username}", username);
			throw;
		}
	}

	public async Task LogoutAsync()
	{
		_logger.LogInformation("Logging out user");

		SecureStorage.Default.Remove(TokenKey);
		SecureStorage.Default.Remove(UsernameKey);
		SecureStorage.Default.Remove(FullNameKey);
		SecureStorage.Default.Remove(RoleKey);
		SecureStorage.Default.Remove(UserIdKey);

		_logger.LogInformation("User logged out successfully");
		await Task.CompletedTask;
	}

	public async Task<string?> GetTokenAsync()
	{
		try
		{
			var token = await SecureStorage.Default.GetAsync(TokenKey);
			_logger.LogDebug("Retrieved token from SecureStorage: {HasToken}", !string.IsNullOrEmpty(token));
			return token;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error retrieving token from SecureStorage");
			return null;
		}
	}

	public async Task<bool> IsAuthenticatedAsync()
	{
		var token = await GetTokenAsync();
		var isAuthenticated = !string.IsNullOrEmpty(token);
		_logger.LogDebug("IsAuthenticated check: {IsAuthenticated}", isAuthenticated);
		return isAuthenticated;
	}

	public async Task<UserDto?> GetCurrentUserAsync()
	{
		try
		{
			_logger.LogDebug("Retrieving current user from SecureStorage");

			var username = await SecureStorage.Default.GetAsync(UsernameKey);
			var fullName = await SecureStorage.Default.GetAsync(FullNameKey);
			var role = await SecureStorage.Default.GetAsync(RoleKey);
			var userIdStr = await SecureStorage.Default.GetAsync(UserIdKey);

			if (string.IsNullOrEmpty(username))
			{
				_logger.LogWarning("No user data found in SecureStorage");
				return null;
			}

			var user = new UserDto
			{
				Id = int.TryParse(userIdStr, out var userId) ? userId : 0,
				Username = username,
				FullName = fullName ?? username,
				Role = role ?? "User",
				IsActive = true,
				Email = string.Empty
			};

			_logger.LogInformation("Retrieved current user: {Username} ({FullName})", user.Username, user.FullName);
			return user;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error retrieving current user from SecureStorage");
			return null;
		}
	}
}
