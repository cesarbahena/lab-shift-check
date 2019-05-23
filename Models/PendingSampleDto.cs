namespace ShiftCheck.Models;

public class PendingSampleDto
{
	public int SampleId { get; set; }
	public int? FolioGrd { get; set; }
	public string Reason { get; set; } = string.Empty;
}
