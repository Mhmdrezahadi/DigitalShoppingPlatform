

namespace DSP.ProductService.Data
{
    public class OrderDetailToReturnDTO
    {
        public ProductToReturnDTO Product { get; set; }
        public Guid ProductId { get; set; }
        public ColorDTO Color { get; set; }
        public Guid ColorId { get; set; }
        public int Count { get; set; }
        public decimal Amount { get; set; }
        public double Discount { get; set; }
    }
}
