using System;

namespace DSP.ProductService.Data
{
    public class FastPricingKeysToReturnDTO
    {
        public Guid FastPricingKeyId { get; set; }
        public string Section { get; set; }
        public string Name { get; set; }
        public string Hint { get; set; }
        public ValueType ValueType { get; set; }
        public FastPricingDDsToReturnDTO FastPricingDD { get; set; }
    }
}
