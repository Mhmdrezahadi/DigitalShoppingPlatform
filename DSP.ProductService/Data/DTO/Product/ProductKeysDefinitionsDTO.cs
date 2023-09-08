using System.Collections.Generic;

namespace DSP.ProductService.Data
{
    public class ProductKeysDefinitionsDTO
    {
        public int CategoryId { get; set; }
        public List<PropertyKeyForCreateDTO> PropertyKeys { get; set; }
    }
}
