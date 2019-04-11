namespace ShiftCheck.Models;

public class ShiftDto
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public TimeSpan StartTime { get; set; }
	public TimeSpan EndTime { get; set; }
	public bool IsActive { get; set; }
	public string TimeRange => $"{StartTime:hh\\:mm} - {EndTime:hh\\:mm}";
}
