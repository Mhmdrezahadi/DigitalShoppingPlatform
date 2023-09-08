using System;
using System.Collections.Generic;

namespace DSP.ProductService.Data
{
    public class FastPricingKeysAndDDsToReturnDTO
    {
        public Guid FastPricingKeyId { get; set; }
        public string Section { get; set; }
        public string Name { get; set; }
        public string Hint { get; set; }
        public ValueType ValueType { get; set; }
        public ICollection<FastPricingDDsToReturnDTO> FastPricingDDs { get; set; }
    }
}
