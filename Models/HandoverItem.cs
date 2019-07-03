using ShiftCheck.ViewModels;


namespace ShiftCheck.Models
{

public partial class HandoverItem : ViewModelBase
{
	public int ExamId { get; set; }
	public int? Folio { get; set; }
	public string ExamName { get; set; } = string.Empty;

	private string _reason = string.Empty;
	public string Reason { get { return _reason; } set { SetProperty(ref _reason, value); } }
}

}
