using System;

namespace DSP.Gateway.Data
{
    public class OrderHistoryDTO
    {
        public decimal Amount { get; set; }
        public DateTime DateTime { get; set; }
        public string PaymentName { get; set; }
        public bool TransactionStatus { get; set; }
        public string Code { get; set; }
    }
}
