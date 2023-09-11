namespace DSP.ProductService.Data
{
    public class ProductSearch
    {
        /// <summary>
        /// جستجو در داخل یک دسته بندی
        /// </summary>
        public Guid? CategoryId { get; set; } = null;

        /// <summary>
        /// متن جستجو
        /// </summary>
        public string SearchText { get; set; }

        /// <summary>
        /// ترتیب مرتب سازی
        /// </summary>
        public ProductSearchOrder? SearchOrder { get; set; }

        /// <summary>
        /// جست و جو بر اساس نوع محصول
        /// </summary>
        public ProductSearchType? ProductType { get; set; }

        /// <summary>
        /// جست و جو در داخل یک برند
        /// </summary>
        public Guid? BrandId { get; set; } = null;

        /// <summary>
        /// جست و جو در داخل یک مدل
        /// </summary>
        public Guid? ModelId { get; set; } = null;
    }
}

namespace DSP.ProductService.Data
{
    /// <summary>
    /// مرتب سازی
    /// </summary>
    public enum ProductSearchOrder
    {
        /// <summary>
        /// جدید ترین
        /// </summary>
        New,
        /// <summary>
        /// گران ترین
        /// </summary>
        Expensive,
        /// <summary>
        /// ارزان ترین
        /// </summary>
        Cheap,
    }
    public enum ProductSearchType
    {
        /// <summary>
        /// محصول نو
        /// </summary>
        New,

        /// <summary>
        /// محصول کارکرده
        /// </summary>
        Used
    }
}