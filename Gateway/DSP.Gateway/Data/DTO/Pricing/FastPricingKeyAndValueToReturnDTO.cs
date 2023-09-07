namespace DSP.Gateway.Data
{
    public class FastPricingKeyAndValueToReturnDTO
    {
        public ValueType ValueType { get; set; }
        public string Section { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
    public enum ValueType
    {
        /// <summary>
        /// منطقی
        /// بلی خیر
        /// </summary>
        Boolean,

        /// <summary>
        /// متنی
        /// </summary>
        Text,

        /// <summary>
        /// عددی
        /// </summary>
        Numeric
    }
    public enum RateType
    {
        /// <summary>
        /// تفریق
        /// </summary>
        Subtract,

        /// <summary>
        /// درصد
        /// </summary>
        Percent
    }

}
