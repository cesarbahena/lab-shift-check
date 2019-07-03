using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ShiftCheck.Models
{

public class CreateShiftHandoverDto
{
	public int ShiftId { get; set; }
	public int UserId { get; set; }
	public DateTime HandoverDate { get; set; }
	public string Notes { get; set; }
	[JsonProperty("pendingExams")]
	public List<PendingSampleDto> PendingExams { get; set; } = new List<PendingSampleDto>();
}

}
