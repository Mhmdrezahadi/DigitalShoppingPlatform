using System;

namespace DSP.ProductService.Data
{
    public class PropertyValueForCreateDTO
    {
        public Guid? Id { get; set; }
        public string Value { get; set; }
        public Guid PropertyKeyId { get; set; }
    }
}
