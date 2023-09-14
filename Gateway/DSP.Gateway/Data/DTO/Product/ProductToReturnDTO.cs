
namespace DSP.Gateway.Data
{
    public class ProductToReturnDTO
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
        public List<PropertyKeyDTO> PropertyKeys { get; set; }
        public List<PropertyValueDTO> PropertyValues { get; set; }
        /// <summary>
        /// موجود - ناموجود - مخفی
        /// </summary>
        public Status? Status { get; set; }
        public string Warranty { get; set; }
        /// <summary>
        /// متن توضیحات مهلت تست دستگاه
        /// </summary>
        public string ProductTestDescription { get; set; }
        public ICollection<ColorDTO> Colors { get; set; }
        public Guid CategoryId { get; set; }
        public Guid? BrandId { get; set; }
        public CategoryToReturnDTO Category { get; set; }
        public CategoryToReturnDTO Brand { get; set; }
        public CategoryToReturnDTO Model { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
