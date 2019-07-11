namespace ShiftCheck.Models
{
    public enum HandoverSubmissionStatus
    {
        Created,
        Rejected,
        Uncertain
    }

    public sealed class HandoverSubmissionResult
    {
        public HandoverSubmissionStatus Status { get; private set; }
        public ShiftHandoverDto Handover { get; private set; }
        public string Message { get; private set; }

        private HandoverSubmissionResult(HandoverSubmissionStatus status, ShiftHandoverDto handover, string message)
        {
            Status = status;
            Handover = handover;
            Message = message;
        }

        public static HandoverSubmissionResult Created(ShiftHandoverDto handover)
        {
            return new HandoverSubmissionResult(HandoverSubmissionStatus.Created, handover, string.Empty);
        }

        public static HandoverSubmissionResult Rejected(string message)
        {
            return new HandoverSubmissionResult(HandoverSubmissionStatus.Rejected, null, message);
        }

        public static HandoverSubmissionResult Uncertain()
        {
            return new HandoverSubmissionResult(HandoverSubmissionStatus.Uncertain, null,
                "No se pudo confirmar la entrega. Revise las entregas en Hub antes de crear otra.");
        }
    }
}
