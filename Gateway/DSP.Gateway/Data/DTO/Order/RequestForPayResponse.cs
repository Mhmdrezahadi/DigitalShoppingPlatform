
namespace DSP.Gateway.Data
{
    public class RequestForPayResponse
    {
        public string MerchantId { get; set; }
        public PaymentResponse PaymentResponse { get; set; }

    }
    public class PaymentResponse
    {
        public String Authority { set; get; }
        public int Status { set; get; }
        public String PaymentURL { set; get; }

    }
}
