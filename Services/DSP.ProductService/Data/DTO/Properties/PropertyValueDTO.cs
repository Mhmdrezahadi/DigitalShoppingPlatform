using System;

namespace DSP.ProductService.Data
{
    public class PropertyValueDTO
    {
        public Guid PropertyValueId { get; set; }
        public string Value { get; set; }
        public Guid PropertyKeyId { get; set; }

    }
}
