
namespace DSP.ProductService.Data
{
    public class OrderToReturnDTO
    {
        public Guid OrderId { get; set; }
        public DateTime DT { get; set; }
        public UserToReturnDTO User { get; set; }
        public AddressToReturnDTO Address { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatusDTO OrderStatus { get; set; }
        public string Code { get; set; }
        public double Discount { get; set; }
        public decimal Tax { get; set; }
        public string TrackingCode { get; set; }
    }
}
