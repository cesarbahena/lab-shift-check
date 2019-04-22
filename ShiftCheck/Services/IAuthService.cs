using ShiftCheck.Models;

namespace ShiftCheck.Services;

public interface IAuthService
{
	Task<LoginResponse?> LoginAsync(string username, string password);
	Task LogoutAsync();
	Task<string?> GetTokenAsync();
	Task<bool> IsAuthenticatedAsync();
	Task<UserDto?> GetCurrentUserAsync();
}
