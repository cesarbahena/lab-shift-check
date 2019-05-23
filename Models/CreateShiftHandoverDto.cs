namespace ShiftCheck.Models;

public class CreateShiftHandoverDto
{
	public int ShiftId { get; set; }
	public int UserId { get; set; }
	public DateTime HandoverDate { get; set; }
	public string? Notes { get; set; }
	public List<PendingSampleDto> PendingSamples { get; set; } = new();
}
