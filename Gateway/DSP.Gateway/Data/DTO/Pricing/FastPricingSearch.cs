using DSP.Gateway.Utilities;

namespace DSP.Gateway.Data
{
    public class FastPricingSearch
    {
        /// <summary>
        /// متن جستجو
        /// </summary>
        public string SearchText { get; set; }
        /// <summary>
        ///  دسته بندی
        /// </summary>
        public int? CategoryId { get; set; }
        /// <summary>
        ///  برند
        /// </summary>
        public int? BrandId { get; set; }
        /// <summary>
        /// مدل
        /// </summary>
        public int? ModelId { get; set; }
        /// <summary>
        /// مرتب سازی
        /// </summary>
        public SearchOrder? PricingSearchOrder { get; set; }
    }
}
