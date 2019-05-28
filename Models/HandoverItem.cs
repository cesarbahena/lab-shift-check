using CommunityToolkit.Mvvm.ComponentModel;

namespace ShiftCheck.Models;

public partial class HandoverItem : ObservableObject
{
	public int ExamId { get; init; }
	public int? Folio { get; init; }
	public string ExamName { get; init; } = string.Empty;

	[ObservableProperty]
	private string reason = "Pendiente de liberación";
}
