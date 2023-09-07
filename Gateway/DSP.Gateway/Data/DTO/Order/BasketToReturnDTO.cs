using System;
using System.Collections.Generic;

namespace DSP.Gateway.Data
{
    public class BasketToReturnDTO
    {
        public Guid BasketId { get; set; }
        public decimal Price { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalPrice { get; set; }
        public List<BasketDetailToReturnDTO> BasketDetails { get; set; }
    }
}
