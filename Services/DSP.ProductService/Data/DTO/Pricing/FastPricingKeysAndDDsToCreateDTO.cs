using System;
using System.Collections.Generic;

namespace DSP.ProductService.Data
{
    public class FastPricingKeysAndDDsToCreateDTO
    {
        public Guid? FastPricingKeyId { get; set; }
        public string Section { get; set; }
        public string Name { get; set; }
        public string Hint { get; set; }
        public ValueType ValueType { get; set; }
        public ICollection<FastPricingDDsToCreateDTO> FastPricingDDs { get; set; }

    }
}
