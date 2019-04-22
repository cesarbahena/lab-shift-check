using System.Text;
using System.Text.Json;
using ShiftCheck.Models;

namespace ShiftCheck.Services;

public class AuthService : IAuthService
{
	private readonly IHttpClientFactory _httpClientFactory;
	private const string TokenKey = "auth_token";
	private const string UserKey = "current_user";

	public AuthService(IHttpClientFactory httpClientFactory)
	{
		_httpClientFactory = httpClientFactory;
	}

	public async Task<LoginResponse?> LoginAsync(string username, string password)
	{
		try
		{
			var client = _httpClientFactory.CreateClient("QuimiOSHub");
			var request = new LoginRequest { Username = username, Password = password };
			var json = JsonSerializer.Serialize(request);
			var content = new StringContent(json, Encoding.UTF8, "application/json");

			var response = await client.PostAsync("/api/auth/login", content);

			if (!response.IsSuccessStatusCode)
				return null;

			var responseJson = await response.Content.ReadAsStringAsync();
			var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseJson,
				new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

			if (loginResponse != null)
			{
				await SecureStorage.SetAsync(TokenKey, loginResponse.Token);
				var userDto = new UserDto
				{
					Username = loginResponse.Username,
					FullName = loginResponse.FullName,
					Role = loginResponse.Role
				};
				await SecureStorage.SetAsync(UserKey, JsonSerializer.Serialize(userDto));
			}

			return loginResponse;
		}
		catch
		{
			return null;
		}
	}

	public async Task LogoutAsync()
	{
		SecureStorage.Remove(TokenKey);
		SecureStorage.Remove(UserKey);
		await Task.CompletedTask;
	}

	public async Task<string?> GetTokenAsync()
	{
		return await SecureStorage.GetAsync(TokenKey);
	}

	public async Task<bool> IsAuthenticatedAsync()
	{
		var token = await GetTokenAsync();
		return !string.IsNullOrEmpty(token);
	}

	public async Task<UserDto?> GetCurrentUserAsync()
	{
		var userJson = await SecureStorage.GetAsync(UserKey);
		if (string.IsNullOrEmpty(userJson))
			return null;

		return JsonSerializer.Deserialize<UserDto>(userJson);
	}
}
