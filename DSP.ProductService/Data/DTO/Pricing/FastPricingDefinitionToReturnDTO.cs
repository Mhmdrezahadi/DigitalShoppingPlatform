using System;
using System.Collections.Generic;

namespace DSP.ProductService.Data
{
    public class FastPricingDefinitionToReturnDTO
    {
        public Guid Id { get; set; }
        public CategoryToReturnDTO Category { get; set; }
        public CategoryToReturnDTO Brand { get; set; }
        public CategoryToReturnDTO Model { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public List<FastPricingKeysAndDDsToReturnDTO> KeysAndDDs { get; set; }
    }
}
