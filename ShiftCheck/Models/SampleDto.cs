namespace ShiftCheck.Models;

public class SampleDto
{
	public int Id { get; set; }
	public int? FolioGrd { get; set; }
	public int? ClienteGrd { get; set; }
	public DateTime? FechaGrd { get; set; }
	public DateTime? FechaRecep { get; set; }
	public int? PacienteGrd { get; set; }
	public int? EstPerGrd { get; set; }
	public string? Label1 { get; set; }
	public string? Label3 { get; set; }
	public string? Maquilador { get; set; }
	public DateTime? FecCapRes { get; set; }
	public DateTime? FecLibera { get; set; }
	public string? SucProc { get; set; }
	public DateTime? FecNac { get; set; }
	public DateTime CreatedAt { get; set; }
	public bool IsCompleted => FecLibera.HasValue;
	public bool IsPending => !FecLibera.HasValue;
}
