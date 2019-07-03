using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ShiftCheck.Models
{

public class ShiftHandoverDto
{
	public int Id { get; set; }
	public int ShiftId { get; set; }
	public string ShiftName { get; set; } = string.Empty;
	public int UserId { get; set; }
	public string UserName { get; set; } = string.Empty;
	public DateTime HandoverDate { get; set; }
	public string Notes { get; set; }
	[JsonProperty("pendingExamsCount")]
	public int PendingExamsCount { get; set; }
	[JsonProperty("pendingExams")]
	public List<PendingSampleDto> PendingExams { get; set; } = new List<PendingSampleDto>();
}

}
