using ShiftCheck.Models;

namespace ShiftCheck.Services;

public interface IApiService
{
	Task<List<SampleDto>> GetPendingSamplesAsync();
	Task<List<ShiftDto>> GetShiftsAsync();
	Task<List<UserDto>> GetUsersAsync();
	Task<ShiftHandoverDto?> CreateShiftHandoverAsync(CreateShiftHandoverDto handover);
	Task<List<ShiftHandoverDto>> GetShiftHandoversAsync(DateTime? startDate = null, DateTime? endDate = null);
}
