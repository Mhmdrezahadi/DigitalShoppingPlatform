using System;
using System.Collections.Generic;

namespace DSP.ProductService.Data
{
    public class FastPricingDefinitionToCreateDTO
    {
        public int CategoryId { get; set; }
        public Guid ProductId { get; set; }
        public ICollection<FastPricingKeysAndDDsToCreateDTO> FastPricingKeysAndDDs { get; set; }
    }
}
