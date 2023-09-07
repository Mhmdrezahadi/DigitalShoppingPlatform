using System;

namespace DSP.Gateway.Data
{
    public class BasketCheckOutDTO
    {
        public Guid BasketId { get; set; }
        public Guid? PaymentGateWayId { get; set; }
    }
}
