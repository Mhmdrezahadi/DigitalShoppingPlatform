using System.Collections.Generic;

namespace DSP.Gateway.Data
{
    public class ProductKeysDefinitionsDTO
    {
        public int CategoryId { get; set; }
        public List<PropertyKeyForCreateDTO> PropertyKeys { get; set; }
    }
}
