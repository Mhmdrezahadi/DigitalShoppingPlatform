using System.Collections.Generic;

namespace DSP.ProductService.Data
{
    public class ProductKeysDefinitionsDTO
    {
        public Guid CategoryId { get; set; }
        public List<PropertyKeyForCreateDTO> PropertyKeys { get; set; }
    }
}
