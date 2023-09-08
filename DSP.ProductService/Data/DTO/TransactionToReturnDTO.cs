using DSP.ProductService.Data;
using System;

namespace DSP.ProductService.Data
{
    public class TransactionToReturnDTO
    {
        public Guid Id { get; set; }
        public DateTime DT { get; set; }
        public string Gate { get; set; }
        public bool Status { get; set; }
        public string Code { get; set; }
        public decimal Amount { get; set; }
        public PaymentType PaymentType { get; set; }
        public UserToReturnDTO User { get; set; }

    }
}
