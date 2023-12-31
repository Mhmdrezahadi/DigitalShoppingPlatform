﻿

namespace DSP.ProductService.Data
{
    public class TransactionProductDTO
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        /// <summary>
        /// توضیحات کامل برای دستگاه های نو
        /// دستگاه های دست دوم معمولا توضیحات تکمیلی را ندارند
        /// </summary>
        public string Description { get; set; }
        public decimal Price { get; set; }
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
        public Guid CategoryId { get; set; }
        public int Count { get; set; }
    }
}
