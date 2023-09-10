using System;

namespace DSP.ProductService.Data
{
    public class FastPricingDDsToCreateDTO
    {
        public Guid? Id { get; set; }
        public string Label { get; set; }
        public double? MinRate { get; set; }
        public double? MaxRate { get; set; }
        public string ErrorTitle { get; set; }
        public string ErrorDiscription { get; set; }
    }
}
