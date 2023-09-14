using System.Collections.Generic;

namespace DSP.Gateway.Data
{
    public class ProductKeysDefinitionsDTO
    {
        public Guid CategoryId { get; set; }
        public List<PropertyKeyForCreateDTO> PropertyKeys { get; set; }
    }
}
