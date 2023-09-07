
namespace DSP.Gateway.Data
{
    public class MyOrderToReturnDTO
    {
        public Guid OrderId { get; set; }
        public string Code { get; set; }
        public DateTime DT { get; set; }
        public List<ProductInOrderToReturnDTO> ProductsInOrder { get; set; }
        public OrderStatusDTO OrderStatus { get; set; }
        public decimal Price { get; set; }
        public AddressToReturnDTO Address { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Tax { get; set; }
        public int Count { get; set; }
    }
    public enum OrderStatusDTO
    {
        /// <summary>
        /// درحال بررسی
        /// </summary>
        Pending,

        /// <summary>
        /// در حال پردازش
        /// </summary>
        InProcess,

        /// <summary>
        /// تحویل به سفیر تل بال
        /// </summary>
        AgentDelivered,

        /// <summary>
        /// تحویل به مشتری
        /// </summary>
        CustomerDelivered,

        /// <summary>
        /// انصراف از فروش توسط فروشنده
        /// </summary>
        Canceled,

        /// <summary>
        /// مرجوعی
        /// </summary>
        Returned
    }
}
