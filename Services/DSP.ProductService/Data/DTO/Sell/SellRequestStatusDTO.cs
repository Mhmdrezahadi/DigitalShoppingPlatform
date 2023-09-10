

namespace DSP.ProductService.Data
{
    public class SellRequestStatusDTO
    {
        public string StatusDescription { get; set; }
        public SellRequestStatus SellRequestStatus { get; set; }
        public decimal AgreedPrice { get; set; }
    }
}
