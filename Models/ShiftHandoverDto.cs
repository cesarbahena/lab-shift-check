using System.Text.Json.Serialization;

namespace ShiftCheck.Models;

public class ShiftHandoverDto
{
	public int Id { get; set; }
	public int ShiftId { get; set; }
	public string ShiftName { get; set; } = string.Empty;
	public int UserId { get; set; }
	public string UserName { get; set; } = string.Empty;
	public DateTime HandoverDate { get; set; }
	public string? Notes { get; set; }
	[JsonPropertyName("pendingExamsCount")]
	public int PendingExamsCount { get; set; }
	[JsonPropertyName("pendingExams")]
	public List<PendingSampleDto> PendingExams { get; set; } = new();
}
