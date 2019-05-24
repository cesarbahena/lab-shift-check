namespace ShiftCheck.Models;

public class SampleDto
{
	public int Id { get; set; }
	public DateTime? CreatedAt { get; set; }
	public DateTime? ReceivedAt { get; set; }
	public int? Folio { get; set; }
	public int? ClientId { get; set; }
	public int? PatientId { get; set; }
	public int? ExamId { get; set; }
	public string? ExamName { get; set; }
	public DateTime? ProcessedAt { get; set; }
	public DateTime? ValidatedAt { get; set; }
	public string? Location { get; set; }
	public string? Outsourcer { get; set; }
	public string? Priority { get; set; }
	public DateTime? BirthDate { get; set; }
	public bool IsCompleted => ValidatedAt.HasValue;
	public bool IsPending => !ValidatedAt.HasValue;
}
