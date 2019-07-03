using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using ShiftCheck.Models;

namespace ShiftCheck.Services
{

public class ApiService : IApiService
{
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly IAuthService _authService;
	private readonly ILogger<ApiService> _logger;

	public ApiService(IHttpClientFactory httpClientFactory, IAuthService authService, ILogger<ApiService> logger)
	{
		_httpClientFactory = httpClientFactory;
		_authService = authService;
		_logger = logger;
	}

	private async Task<HttpClient> GetAuthenticatedClientAsync()
	{
		var client = _httpClientFactory.CreateClient("QuimiOSHub");
		var token = await _authService.GetTokenAsync();

		if (!string.IsNullOrEmpty(token))
		{
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			_logger.LogDebug("Added Bearer token to request headers");
		}
		else
		{
			_logger.LogWarning("No authentication token available for API request");
		}

		return client;
	}

	public async Task<List<SampleDto>> GetPendingSamplesAsync()
	{
		try
		{
			_logger.LogInformation("Fetching pending samples from API");

			var client = await GetAuthenticatedClientAsync();
			var response = await client.GetAsync("exams/pending");

			_logger.LogInformation("GetPendingSamples response: {StatusCode}", response.StatusCode);

			if (!response.IsSuccessStatusCode)
			{
				var errorBody = await response.Content.ReadAsStringAsync();
				_logger.LogWarning("Failed to fetch pending samples: {StatusCode}, Body: {ErrorBody}",
					response.StatusCode, errorBody);
				throw new HttpRequestException("No se pudieron cargar los exámenes pendientes.");
			}

			var json = await response.Content.ReadAsStringAsync();
			_logger.LogDebug("Received JSON response: {Length} characters", json.Length);

			var samples = HubJson.Read<List<SampleDto>>(json) ?? new List<SampleDto>();

			_logger.LogInformation("Successfully fetched {Count} pending samples", samples.Count);
			return samples;
		}
		catch (HttpRequestException ex)
		{
			_logger.LogError(ex, "Network error while fetching pending samples");
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Unexpected error while fetching pending samples");
			throw;
		}
	}

	public async Task<List<ShiftDto>> GetShiftsAsync()
	{
		try
		{
			_logger.LogInformation("Fetching shifts from API");

			var client = await GetAuthenticatedClientAsync();
			var response = await client.GetAsync("shifts");

			_logger.LogInformation("GetShifts response: {StatusCode}", response.StatusCode);

			if (!response.IsSuccessStatusCode)
			{
				var errorBody = await response.Content.ReadAsStringAsync();
				_logger.LogWarning("Failed to fetch shifts: {StatusCode}, Body: {ErrorBody}",
					response.StatusCode, errorBody);
				throw new HttpRequestException("No se pudieron cargar los turnos.");
			}

			var json = await response.Content.ReadAsStringAsync();
			_logger.LogDebug("Received JSON response: {Length} characters", json.Length);

			var shifts = HubJson.Read<List<ShiftDto>>(json) ?? new List<ShiftDto>();

			_logger.LogInformation("Successfully fetched {Count} shifts", shifts.Count);
			return shifts;
		}
		catch (HttpRequestException ex)
		{
			_logger.LogError(ex, "Network error while fetching shifts");
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Unexpected error while fetching shifts");
			throw;
		}
	}

	public async Task<List<UserDto>> GetUsersAsync()
	{
		try
		{
			_logger.LogInformation("Fetching active users from API");

			var client = await GetAuthenticatedClientAsync();
			var response = await client.GetAsync("users?isActive=true");

			_logger.LogInformation("GetUsers response: {StatusCode}", response.StatusCode);

			if (!response.IsSuccessStatusCode)
			{
				var errorBody = await response.Content.ReadAsStringAsync();
				_logger.LogWarning("Failed to fetch users: {StatusCode}, Body: {ErrorBody}",
					response.StatusCode, errorBody);
				return new List<UserDto>();
			}

			var json = await response.Content.ReadAsStringAsync();
			_logger.LogDebug("Received JSON response: {Length} characters", json.Length);

			var users = HubJson.Read<List<UserDto>>(json) ?? new List<UserDto>();

			_logger.LogInformation("Successfully fetched {Count} active users", users.Count);
			return users;
		}
		catch (HttpRequestException ex)
		{
			_logger.LogError(ex, "Network error while fetching users");
			return new List<UserDto>();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Unexpected error while fetching users");
			return new List<UserDto>();
		}
	}

	public async Task<ShiftHandoverDto> CreateShiftHandoverAsync(CreateShiftHandoverDto handover)
	{
		try
		{
			_logger.LogInformation("Creating shift handover for shift {ShiftId} with {SampleCount} samples",
				handover.ShiftId, handover.PendingExams.Count);

			var client = await GetAuthenticatedClientAsync();
			var json = HubJson.Write(handover);
			var content = new StringContent(json, Encoding.UTF8, "application/json");

			var response = await client.PostAsync("shifthandovers", content);

			_logger.LogInformation("CreateShiftHandover response: {StatusCode}", response.StatusCode);

			if (!response.IsSuccessStatusCode)
			{
				var errorBody = await response.Content.ReadAsStringAsync();
				_logger.LogWarning("Failed to create shift handover: {StatusCode}, Body: {ErrorBody}",
					response.StatusCode, errorBody);
				System.Diagnostics.Debug.WriteLine($"[SHIFTCHECK ERROR] Status: {response.StatusCode}, Body: {errorBody}");
				return null;
			}

			var responseJson = await response.Content.ReadAsStringAsync();
			_logger.LogDebug("Received response: {Length} characters", responseJson.Length);

			var result = HubJson.Read<ShiftHandoverDto>(responseJson);

			_logger.LogInformation("Successfully created shift handover with ID {HandoverId}", result?.Id);
			return result;
		}
		catch (HttpRequestException ex)
		{
			_logger.LogError(ex, "Network error while creating shift handover");
			return null;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Unexpected error while creating shift handover");
			return null;
		}
	}

	public async Task<List<ShiftHandoverDto>> GetShiftHandoversAsync(DateTime? startDate = null, DateTime? endDate = null)
	{
		try
		{
			_logger.LogInformation("Fetching shift handovers (StartDate: {StartDate}, EndDate: {EndDate})",
				startDate?.ToString("yyyy-MM-dd") ?? "null",
				endDate?.ToString("yyyy-MM-dd") ?? "null");

			var client = await GetAuthenticatedClientAsync();
			var url = "shifthandovers";

			if (startDate.HasValue || endDate.HasValue)
			{
				var queryParams = new List<string>();
				if (startDate.HasValue)
					queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
				if (endDate.HasValue)
					queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");
				url += "?" + string.Join("&", queryParams);
			}

			_logger.LogDebug("Request URL: {Url}", url);

			var response = await client.GetAsync(url);

			_logger.LogInformation("GetShiftHandovers response: {StatusCode}", response.StatusCode);

			if (!response.IsSuccessStatusCode)
			{
				var errorBody = await response.Content.ReadAsStringAsync();
				_logger.LogWarning("Failed to fetch shift handovers: {StatusCode}, Body: {ErrorBody}",
					response.StatusCode, errorBody);
				return new List<ShiftHandoverDto>();
			}

			var json = await response.Content.ReadAsStringAsync();
			_logger.LogDebug("Received JSON response: {Length} characters", json.Length);

			var handovers = HubJson.Read<List<ShiftHandoverDto>>(json) ?? new List<ShiftHandoverDto>();

			_logger.LogInformation("Successfully fetched {Count} shift handovers", handovers.Count);
			return handovers;
		}
		catch (HttpRequestException ex)
		{
			_logger.LogError(ex, "Network error while fetching shift handovers");
			return new List<ShiftHandoverDto>();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Unexpected error while fetching shift handovers");
			return new List<ShiftHandoverDto>();
		}
	}
}

}
