using System;

namespace DSP.ProductService.Data
{
    public class CheckOutResultDTO
    {
        public string TrackingCode { get; set; }
        public AddressToReturnDTO Address { get; set; }
        public decimal Price { get; set; }
        public DateTime DT { get; set; }
        public bool Status { get; set; }

    }
}
