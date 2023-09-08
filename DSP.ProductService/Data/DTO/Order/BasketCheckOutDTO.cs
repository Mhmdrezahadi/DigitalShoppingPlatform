using System;

namespace DSP.ProductService.Data
{
    public class BasketCheckOutDTO
    {
        public Guid BasketId { get; set; }
        public Guid? PaymentGateWayId { get; set; }
    }
}
