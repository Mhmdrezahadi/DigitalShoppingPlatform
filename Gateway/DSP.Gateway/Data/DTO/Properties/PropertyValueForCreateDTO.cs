using System;

namespace DSP.Gateway.Data
{
    public class PropertyValueForCreateDTO
    {
        public Guid? Id { get; set; }
        public string Value { get; set; }
        public Guid PropertyKeyId { get; set; }
    }
}
