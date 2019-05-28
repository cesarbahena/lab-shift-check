using System.Text.Json.Serialization;

namespace ShiftCheck.Models;

public class PendingSampleDto
{
	[JsonPropertyName("examId")]
	public int ExamId { get; set; }
	public int? Folio { get; set; }
	public string Reason { get; set; } = string.Empty;
}
