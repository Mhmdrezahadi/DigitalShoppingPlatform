using System;

namespace DSP.ProductService.Data
{
    public class UserPaymentToReturnDTO
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }
        public DateTime DT { get; set; }
        public string Gate { get; set; }
        public bool Status { get; set; }
        public string Code { get; set; }
    }
}
