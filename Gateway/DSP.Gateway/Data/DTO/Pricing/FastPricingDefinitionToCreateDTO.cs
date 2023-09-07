using System;
using System.Collections.Generic;

namespace DSP.Gateway.Data.Pricing
{
    public class FastPricingDefinitionToCreateDTO
    {
        public int CategoryId { get; set; }
        public Guid ProductId { get; set; }
        public ICollection<FastPricingKeysAndDDsToCreateDTO> FastPricingKeysAndDDs { get; set; }
    }
}
