namespace BackMange.DTO
{
    public class EcpayPaymentDTO
    {
        public string? CustomField1 { get; set; }
        public int? CustomField2 { get; set; }
        public int? CustomField3 { get; set; }
        public int? CustomField4 { get; set; }
        public string? ItemName { get; set; }
        public string? TradeDesc { get; set; }
        public int TotalAmount { get; set; }
        public int TaskId { get; set; }
    }
}
