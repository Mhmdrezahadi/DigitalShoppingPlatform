using DSP.ProductService.Utilities;

namespace DSP.ProductService.Data
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
        public Guid? CategoryId { get; set; }
        /// <summary>
        ///  برند
        /// </summary>
        public Guid? BrandId { get; set; }
        /// <summary>
        /// مدل
        /// </summary>
        public Guid? ModelId { get; set; }
        /// <summary>
        /// مرتب سازی
        /// </summary>
        public SearchOrder? PricingSearchOrder { get; set; }
    }
}
