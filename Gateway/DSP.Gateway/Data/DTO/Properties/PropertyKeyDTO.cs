

namespace DSP.Gateway.Data
{
    public class PropertyKeyDTO
    {
        public Guid PropertyKeyId { get; set; }
        public string Name { get; set; }
        public KeyType KeyType { get; set; }
    }
    public enum KeyType
    {
        /// <summary>
        /// عددی
        /// </summary>
        Numeric,
        /// <summary>
        /// منطقی
        /// </summary>
        Boolean,
        /// <summary>
        /// متنی
        /// </summary>
        Text
    }
}
