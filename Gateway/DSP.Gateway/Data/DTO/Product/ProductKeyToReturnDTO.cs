

namespace DSP.Gateway.Data
{
    public class ProductKeyToReturnDTO
    {
        public Guid PropertyKeyId { get; set; }
        public string Name { get; set; }
        public KeyType KeyType { get; set; }
        public ProductValueToReturnDTO ProductValue { get; set; }
    }
}
