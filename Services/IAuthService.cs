using ShiftCheck.Models;

namespace ShiftCheck.Services;

/// <summary>
/// Service for managing user authentication and authorization
/// </summary>
public interface IAuthService
{
	/// <summary>
	/// Authenticates a user with username and password
	/// </summary>
	Task<LoginResponse?> LoginAsync(string username, string password);

	/// <summary>
	/// Logs out the current user and clears stored credentials
	/// </summary>
	Task LogoutAsync();

	/// <summary>
	/// Retrieves the current authentication token
	/// </summary>
	Task<string?> GetTokenAsync();

	/// <summary>
	/// Checks if a user is currently authenticated
	/// </summary>
	Task<bool> IsAuthenticatedAsync();

	/// <summary>
	/// Gets the currently authenticated user information
	/// </summary>
	Task<UserDto?> GetCurrentUserAsync();
}
