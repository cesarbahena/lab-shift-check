using Newtonsoft.Json;

namespace ShiftCheck.Models
{

public class PendingSampleDto
{
	[JsonProperty("examId")]
	public int ExamId { get; set; }
	public int? Folio { get; set; }
	public string Reason { get; set; } = string.Empty;
}

}
