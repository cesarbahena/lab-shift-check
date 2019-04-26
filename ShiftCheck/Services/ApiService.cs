using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ShiftCheck.Models;

namespace ShiftCheck.Services;

public class ApiService : IApiService
{
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly IAuthService _authService;

	public ApiService(IHttpClientFactory httpClientFactory, IAuthService authService)
	{
		_httpClientFactory = httpClientFactory;
		_authService = authService;
	}

	private async Task<HttpClient> GetAuthenticatedClientAsync()
	{
		var client = _httpClientFactory.CreateClient("QuimiOSHub");
		var token = await _authService.GetTokenAsync();

		if (!string.IsNullOrEmpty(token))
		{
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		}

		return client;
	}

	public async Task<List<SampleDto>> GetPendingSamplesAsync()
	{
		try
		{
			var client = await GetAuthenticatedClientAsync();
			var response = await client.GetAsync("/api/samples/pending");

			if (!response.IsSuccessStatusCode)
				return new List<SampleDto>();

			var json = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<List<SampleDto>>(json,
				new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<SampleDto>();
		}
		catch
		{
			return new List<SampleDto>();
		}
	}

	public async Task<List<ShiftDto>> GetShiftsAsync()
	{
		try
		{
			var client = await GetAuthenticatedClientAsync();
			var response = await client.GetAsync("/api/shifts");

			if (!response.IsSuccessStatusCode)
				return new List<ShiftDto>();

			var json = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<List<ShiftDto>>(json,
				new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ShiftDto>();
		}
		catch
		{
			return new List<ShiftDto>();
		}
	}

	public async Task<List<UserDto>> GetUsersAsync()
	{
		try
		{
			var client = await GetAuthenticatedClientAsync();
			var response = await client.GetAsync("/api/users?isActive=true");

			if (!response.IsSuccessStatusCode)
				return new List<UserDto>();

			var json = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<List<UserDto>>(json,
				new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<UserDto>();
		}
		catch
		{
			return new List<UserDto>();
		}
	}

	public async Task<ShiftHandoverDto?> CreateShiftHandoverAsync(CreateShiftHandoverDto handover)
	{
		try
		{
			var client = await GetAuthenticatedClientAsync();
			var json = JsonSerializer.Serialize(handover);
			var content = new StringContent(json, Encoding.UTF8, "application/json");

			var response = await client.PostAsync("/api/shifthandovers", content);

			if (!response.IsSuccessStatusCode)
				return null;

			var responseJson = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<ShiftHandoverDto>(responseJson,
				new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
		}
		catch
		{
			return null;
		}
	}

	public async Task<List<ShiftHandoverDto>> GetShiftHandoversAsync(DateTime? startDate = null, DateTime? endDate = null)
	{
		try
		{
			var client = await GetAuthenticatedClientAsync();
			var url = "/api/shifthandovers";

			if (startDate.HasValue || endDate.HasValue)
			{
				var queryParams = new List<string>();
				if (startDate.HasValue)
					queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
				if (endDate.HasValue)
					queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");
				url += "?" + string.Join("&", queryParams);
			}

			var response = await client.GetAsync(url);

			if (!response.IsSuccessStatusCode)
				return new List<ShiftHandoverDto>();

			var json = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<List<ShiftHandoverDto>>(json,
				new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ShiftHandoverDto>();
		}
		catch
		{
			return new List<ShiftHandoverDto>();
		}
	}
}
