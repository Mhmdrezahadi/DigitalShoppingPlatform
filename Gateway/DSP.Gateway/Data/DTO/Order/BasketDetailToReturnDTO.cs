

namespace DSP.Gateway.Data
{
    public class BasketDetailToReturnDTO
    {

        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        /// <summary>
        /// توضیحات کامل برای دستگاه های نو
        /// دستگاه های دست دوم معمولا توضیحات تکمیلی را ندارند
        /// </summary>
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPrice { get; set; }
        /// <summary>
        /// خلاصه وضعیت برای دستگاه های دست دوم
        /// خلاصه توضیحات برای دستگاه های نو هم میتواند باشد
        /// </summary>
        public string About { get; set; }
        public double Discount { get; set; }
        public string Code { get; set; }
        public ProductType ProductType { get; set; }
        public List<ProductImageToReturnDTO> Images { get; set; }
        /// <summary>
        /// موجود - ناموجود - مخفی
        /// </summary>
        public Status? Status { get; set; }
        public ColorDTO Color { get; set; }
        public int CategoryId { get; set; }
        public int Count { get; set; }
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
        Used,

        /// <summary>
        /// لوازم جانبی
        /// </summary>
        Accessory
    }
    public enum Status
    {
        /// <summary>
        /// محصول موجود
        /// </summary>
        Available,

        /// <summary>
        /// محصول ناموجود
        /// </summary>
        UnAvailable,

        /// <summary>
        /// مخفی از دید کاربر
        /// </summary>
        Hidden
    }
}
