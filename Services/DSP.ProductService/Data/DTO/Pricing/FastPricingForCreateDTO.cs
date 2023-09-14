using System.Collections.Generic;

namespace DSP.ProductService.Data
{
    public class FastPricingForCreateDTO
    {
        public Guid CategoryId { get; set; }
        public ICollection<FastPricingValueToSetDTO> FastPricingValues { get; set; }
    }
}
