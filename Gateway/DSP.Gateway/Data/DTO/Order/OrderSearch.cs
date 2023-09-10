using DSP.Gateway.Utilities;

namespace DSP.Gateway.Data
{
    public class OrderSearch
    {
        /// <summary>
        /// متن جستجو
        /// </summary>
        public string SearchText { get; set; }
        /// <summary>
        /// مرتب سازی
        /// </summary>
        public SearchOrder? OrderSearchOrder { get; set; }
        /// <summary>
        /// وضعیت سفارش
        /// </summary>
        public OrderSearchStatusDTO? OrderStatusDTO { get; set; }
    }
    public enum OrderSearchStatusDTO
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
        /// تحویل به سفیر 
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
