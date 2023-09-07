using DSP.Gateway.Entities;

namespace DSP.Gateway.Data
{
    public class PaymentDTO
    {
        public Guid OrderId { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
        public DateTime DT { get; set; }
        public string Authority { get; set; }
        public String MerchantID { get; set; }
        public bool IsSuccess { get; set; }
        public decimal Amount { get; set; }
        public string GateWayName { get; set; }

        /// <summary>
        ///  شماره تراکنش
        /// </summary>
        public string RefID { get; set; }
        /// <summary>
        /// کد پیگیری
        /// </summary>
        public string TrackingCode { get; set; }
        public int Status { get; set; }

        /// <summary>
        /// (پن شماره کارت واریز کننده(چندرقم وسط مخفی 
        /// </summary>
        public string CardPan { get; set; }

        /// <summary>
        /// هش شماره کارت واریز کننده
        /// </summary>
        public string CardHash { get; set; }

        /// <summary>
        /// مقدار کارمزد
        /// </summary>
        public int Fee { get; set; }

        /// <summary>
        /// نوع کارمزد
        /// </summary>
        public string FeeType { get; set; }

        /// <summary>
        /// نوع تراکنش
        /// </summary>
        public PaymentType PaymentType { get; set; }
    }

}
