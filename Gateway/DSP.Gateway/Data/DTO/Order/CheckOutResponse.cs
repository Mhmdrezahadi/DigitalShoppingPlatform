namespace DSP.Gateway.Data
{
    public class CheckOutResponse
    {
        public string PaymentUrl { get; set; }
        public string TrackingCode { get; set; }
        public string Authority { get; set; }
    }
}
