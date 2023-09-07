using System.Collections.Generic;

namespace DSP.Gateway.Data
{
    public class FastPricingForCreateDTO
    {
        public int CategoryId { get; set; }
        public ICollection<FastPricingValueToSetDTO> FastPricingValues { get; set; }
    }
}
