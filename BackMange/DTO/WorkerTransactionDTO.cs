
namespace BackMange.DTO
{
    public class WorkerTransactionDTO
    {
        public int TransactionId { get; set; }
        public string? TaskTitle { get; set; }
        public int Amount { get; set; }
        public string? Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public int? Rating { get; set; }
        public string? Review { get; set; }
    }
}