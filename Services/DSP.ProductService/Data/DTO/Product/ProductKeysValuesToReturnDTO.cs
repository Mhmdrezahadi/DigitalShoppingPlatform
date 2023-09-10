using System.Collections.Generic;

namespace DSP.ProductService.Data
{
    public class ProductKeysValuesToReturnDTO
    {
        public List<PropertyKeyDTO> PropertyKeys { get; set; }
        public List<PropertyValueDTO> PropertyValues { get; set; }
    }
}
